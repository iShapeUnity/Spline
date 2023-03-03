using Unity.Mathematics;

namespace iShape.Spline {

    [System.Serializable]
    public struct Anchor {
        
        public enum Type {
            point,
            nextPinch,
            prevPinch,
            doublePinch
        }
        
        public Type type;
        
        public float2 NextPoint;
        public float2 PrevPoint;
        public float2 Position;

        public bool IsNextPinchAvailable => type is Type.nextPinch or Type.doublePinch;
        public bool IsPrevPinchAvailable => type is Type.prevPinch or Type.doublePinch;

        public Anchor(float2 Position) {
            this.Position = Position;
            this.PrevPoint = float2.zero;
            this.NextPoint = float2.zero;
            this.type = Type.point;
        }
        
        public Anchor(float2 Position, float2 PrevPoint, float2 NextPoint) {
            this.Position = Position;
            this.PrevPoint = PrevPoint;
            this.NextPoint = NextPoint;
            this.type = Type.doublePinch;
        }
        
        public Anchor(float2 Position, float2 PrevPoint, float2 NextPoint, Type type) {
            this.Position = Position;
            this.PrevPoint = PrevPoint;
            this.NextPoint = NextPoint;
            this.type = type;
        }
    }

}