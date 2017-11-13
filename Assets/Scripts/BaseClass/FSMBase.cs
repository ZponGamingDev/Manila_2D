using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManilaFSM
{
    public abstract class FSMBase<T, U>
        where T : class
        where U : System.IConvertible
    {
        protected class StateTransition
        {
            readonly T _State;
            readonly U _Command;

            public StateTransition(T t, U u)
            {
                _State = t;
                _Command = u;
            }

            // Don't understand
            public override int GetHashCode()
            {
                return 17 + 31 * _State.GetHashCode() + 31 * _Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                StateTransition other = obj as StateTransition;
                //return other != null && this._Current == other._Current && this._Command == other._Command;
                return other != null && this._State == other._State && this._Command.Equals((other._Command));

            }
        }

        public T Current { get; protected set; }
        protected Dictionary<StateTransition, T> transitions = new Dictionary<StateTransition, T>();

        public virtual T GetNext(U command)
        {
            StateTransition transition = new StateTransition(Current, command);
            T nextState;

            if (!transitions.TryGetValue(transition, out nextState))
            {
                throw new System.Exception("Invalid transition: " + Current + " -> " + command);
            }

            return nextState;
        }

        public abstract T MoveNext(U command);
    }
}