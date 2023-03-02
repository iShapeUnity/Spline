
using System;
using Unity.Collections;
using Unity.Mathematics;

namespace iShape.Spline {

    public readonly struct Spline {
    
        private enum Type {
            line,
            cubic,
            quartic
        }
        
        private readonly Type type;
        private readonly float2 pointA;
        private readonly float2 pointB;
        private readonly float2 anchorA;
        private readonly float2 anchorB;
       
        public Spline(float2 pointA, float2 pointB) {
            this.type = Type.line;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = float2.zero;
            this.anchorB = float2.zero;
        }
        
        public Spline(float2 pointA, float2 pointB, float2 anchor) {
            this.type = Type.cubic;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = anchor;
            this.anchorB = float2.zero;
        }
        
        public Spline(float2 pointA, float2 pointB, float2 anchorA, float2 anchorB) {
            this.type = Type.quartic;
            this.pointA = pointA;
            this.pointB = pointB;
            this.anchorA = anchorA;
            this.anchorB = anchorB;
        }

        public float GetLength(int stepCount) {
            var prevPoint = GetPoint(0f);

            float step = 1.0f / stepCount;
            float path = step;
            float length = 0f;

            for (int i = 0; i < stepCount; i++) {
                var nextPoint = GetPoint(path);
                length += math.distance(nextPoint, prevPoint);

                prevPoint = nextPoint;
                path += step;
            }

            return length;
        }
        
        public float2 GetPoint(float k) {
            return type switch {
                Type.line => SplineMath.GetPointFromLine(pointA, pointB, k),
                Type.cubic => SplineMath.GetPointFromCubic(pointA, pointB, anchorA, k),
                Type.quartic => SplineMath.GetPointFromQuartic(pointA, pointB, anchorA, anchorB, k),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        public NativeArray<float2> GetPoints(float stepLength, Allocator allocator) {
            float length = GetLength(20);
            int n = (int)(length / stepLength + 0.5f);
            float s = 1.0f / n;
            float t = 0;
            var result = new NativeArray<float2>(n + 1, allocator);
            
            for (int i = 0; i < n; i++) {
                result[i] = GetPoint(t);
                t += s;
            }
            
            result[n] = GetPoint(t);

            return result;
        }
    }

}
