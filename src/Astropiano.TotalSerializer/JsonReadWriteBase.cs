using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Astropiano.TotalSerializer
{
    public class JsonReadWriteBase<T> where T: struct
    {
        protected readonly char[] TypeDeclaration = "$type".ToCharArray();
        protected readonly char[] IdDeclaration = "$id".ToCharArray();
        protected readonly char[] RefDeclaration = "$ref".ToCharArray();
        protected readonly char[] ValueDeclaration = "$value".ToCharArray();

        protected const char DoubleQuote = '\"';

        private T _currentState;

        protected void SetState(T newState)
        {
            _currentState = newState;
        }
        
        protected T GetState()
        {
            return _currentState;
        }


        protected void AllowState(T state, [CallerMemberName] string callerMember = null)
        {
            if (!Equals(_currentState, state))
            {
                throw new InvalidOperationException($"Execution of '{callerMember}' not allowed in current state {_currentState}");
            }
        }

        protected void AllowArrayState(T[] states, [CallerMemberName] string callerMember = null)
        {
            if (!states.Contains(_currentState))
            {
                throw new InvalidOperationException($"Execution of '{callerMember}' not allowed in current state {_currentState}");
            }
        }
        
        protected void AllowState(params T[] states)
        {
            if (!states.Contains(_currentState))
            {
                throw new InvalidOperationException($"Execution not allowed in current state {_currentState}");
            }
        }

        public bool InState(T state)
        {
            return state.Equals(_currentState);
        }
    }
}
