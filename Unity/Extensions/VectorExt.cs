using System.Collections.Generic;
using UnityEngine;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global

namespace NotFluffy
{
    public static class VectorExt
    {
        public static Vector3 Average(this IEnumerable<Vector3> vectors)
        {
            if(vectors == null)
                return Vector3.zero;

            var count = 0;
            
            var x = 0f;
            var y = 0f;
            var z = 0f;

            foreach (var vector in vectors)
            {
                count++;

                x += vector.x;
                y += vector.y;
                z += vector.z;
            }

            return new Vector3(x, y, z) / count;
        }
    }
}
