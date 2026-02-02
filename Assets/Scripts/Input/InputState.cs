using R3;
using UnityEngine;

namespace ProjectAction.Input
{
    public sealed class InputState
    {
        public ReactiveProperty<Vector2> Move { get; } = new(Vector2.zero);
        public ReactiveProperty<Vector2> Look { get; } = new(Vector2.zero);
        public ReactiveProperty<bool> JumpPressed { get; } = new(false);
        public ReactiveProperty<bool> SprintHeld { get; } = new(false);

        public void Dispose()
        {
            Move.Dispose();
            Look.Dispose();
            JumpPressed.Dispose();
            SprintHeld.Dispose();
        }
    }
}
