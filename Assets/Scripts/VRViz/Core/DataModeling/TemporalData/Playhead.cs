using System;
using System.Collections;
using System.Collections.Generic;

namespace VRViz {
  namespace Core {
    public class Playhead<T> where T : TimeboxedNode {
      public delegate void PlayheadStepHandler(Playhead<T> playhead, float playheadTime, float stepSize);
      public delegate void NodeLifetimeHandler(TimeboxedNode node);
      public delegate void PlayheadRangeHandler(Playhead<T> playhead, float startTime, float endTime);
      public delegate void PlayheadRepeatHandler(Playhead<T> playhead, bool repeatEnabled);

      public event NodeLifetimeHandler OnNodeBorn;
      public event NodeLifetimeHandler OnNodeDied;

      public event PlayheadStepHandler OnStep;
      public event PlayheadRangeHandler OnStartTimeChange;
      public event PlayheadRangeHandler OnEndTimeChange;
      public event PlayheadRepeatHandler OnRepeatToggled;

      public readonly NodeStoreDefault<T> Nodes;

      public float CurrentTime {
        get {
          return m_currentTime;
        }
      }

      public float StartTime {
        get {
          return m_startTime;
        }

        set {
          m_startTime = value;
          PlayheadRangeHandler e = OnStartTimeChange;
          if (e != null)
            e(this, m_startTime, m_endTime);
        }
      }

      public float EndTime {
        get {
          return m_endTime;
        }

        set {
          m_endTime = value;
          PlayheadRangeHandler e = OnEndTimeChange;
          if (e != null)
            e(this, m_startTime, m_endTime);
        }
      }


      public bool Repeat {
        get {
          return m_repeat;
        }

        set {
          m_repeat = value;
          PlayheadRepeatHandler e = OnRepeatToggled;
          if (e != null)
            OnRepeatToggled(this, m_repeat);
        }
      }

      private float m_startTime;
      private float m_endTime;

      private float m_currentTime;

      private bool m_repeat;

      private bool m_initialized;

      public Playhead(NodeStoreDefault<T> nodes, float startTime = 0, float endTime = float.MaxValue, bool repeat = true) {
        m_initialized = false;
        m_currentTime = startTime;

        Nodes = nodes;
        m_startTime = startTime;
        m_endTime = endTime;
        m_repeat = repeat;
      }

      // Used so a listener can setup event listening before we start doing anything.
      // This way there's no special case for having missed an event before getting a chance to listen.
      public void InitializePlayhead() {
        m_initialized = true;

        // Birth appropriate nodes
        birthAndKillNodesForTime(m_currentTime);
      }

      public void StepPlayhead(float stepSize) {
        if (!m_initialized)
          return;

        if (stepSize == 0)
          return;

        // Deal with overruns in either direction
        int direction = stepSize > 0 ? 1 : -1;
        float distanceToStartTime = StartTime - CurrentTime;
        float distanceToEndTime = EndTime - CurrentTime;
        float overrunDistanceToCheck = direction == 1 ? distanceToEndTime : distanceToStartTime;

        if (Math.Abs(overrunDistanceToCheck) < stepSize) {
          if (Repeat) {
            JumpPlayhead(direction == 1 ? StartTime : EndTime);
            stepSize = (stepSize - Math.Abs(overrunDistanceToCheck)) * direction;
          }
          else {
            stepSize = overrunDistanceToCheck;
          }
        }

        // Calculate the new time
        float newTime = CurrentTime + stepSize;
        // Step to new location
        m_currentTime = newTime;

        birthAndKillNodesForTime(m_currentTime);

        PlayheadStepHandler e = OnStep;
        if (e != null)
          e(this, m_currentTime, stepSize);
      }

      // TODO: There's a good opportuinity for optimization here
      //       I can do a lot better than O(Total Number of Nodes) - @Daniel
      private void birthAndKillNodesForTime(float time) {
        foreach (TimeboxedNode node in Nodes.Nodes.Values) {
          bool shouldBeAlive = (m_currentTime >= node.StartTime) && (m_currentTime <= node.EndTime);
          if (node.Alive != shouldBeAlive) {
            node.Alive = !node.Alive;
            NodeLifetimeHandler e = node.Alive ? OnNodeBorn : OnNodeDied;
            if (e != null)
              e(node);
          }
        }
      }

      public void JumpPlayhead(float jumpTime) {
        if (jumpTime > EndTime || jumpTime < StartTime)
          throw new ArgumentOutOfRangeException("jump time must be between start and end time");

        float difference = jumpTime - CurrentTime;
        StepPlayhead(difference);
      }
    }
  }
}