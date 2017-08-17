using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WizardBallisticsCore.BaseClasses;
using WizardBallisticsCore.OneDemGrid;

namespace WBRimanTest {
    public class RmNode : WBOneDemNode {
        public double ro, p, e;
        public static double k = 1.4;
        public static double vyaz = 0.01;

        public double GetE() {
            return p / (ro * (k - 1));
        }
        public double GetP() {
            return ro * e * (k - 1);
        }
        public void InitS() {
            s[1] = ro;
            s[2] = ro * u;
            s[3] = ro * (e + u * u / 2);
        }
        public void InitF() {
            f[1] = ro * u;
            f[2] = ro * u * u + p;
            f[3] = u * (ro * e + ro * u * u / 2 + p);
        }
        public void Synch() {
            e = GetE();
            InitS();
            InitF();
        }
    
        public Vec3 f, s;

        public Vec3 F(Vec3 S) {
            return F(S.v1,S.v2,S.v3);
        }

        public Vec3 F(double s1, double s2, double s3) {
            s = new Vec3(s1, s2, s3);
            ro = s1;
            u = s2 / s1;
            p = (k - 1) * (s3 - s2 * s2 * 0.5 / s1);
            e = GetE();           
            InitF();
            return f;
        }
        public static Vec3 F_func(Vec3 s) {
            var p = (k - 1) * (s[3] - s[2] * s[2] * 0.5 / s[1]);
            return new Vec3(
                s[2],
                s[2] * s[2] / s[1] + p,
                s[2] * (s[3] + p) / s[1]);
        }


    }
    public struct Vec3 {
        public double v1, v2, v3;
        public Vec3(double v1, double v2, double v3) {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
        #region Operators
        public double this[int index] {
            get {
                switch (index) {
                    case 1:
                        return v1;
                    case 2:
                        return v2;
                    case 3:
                        return v3;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (index) {
                    case 1:
                        v1 = value;
                        break;
                    case 2:
                        v2 = value;
                        break;
                    case 3:
                        v3 = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        /// <summary>
		/// Returns the hashcode for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode() {
            return v1.GetHashCode() ^ v2.GetHashCode() ^ v3.GetHashCode();
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to
        /// the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to this instance.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is a <see cref="Vector3D"/> and has the same values as this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) {
            if (obj is Vec3 v) {
                return (v1 == v.v1) && (v2 == v.v2) && (v3 == v.v3);
            }
            return false;
        }

        public static bool AproxEq(Vec3 v, Vec3 u, double tol = 1E-6) {
            return Math.Abs(v.v1 - u.v1) < tol &&
                Math.Abs(v.v2 - u.v2) < tol &&
                Math.Abs(v.v3 - u.v3) < tol;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() {
            return string.Format("({0}, {1}, {2})", v1, v2, v3);
        }
        #endregion

        #region Comparison Operators
        /// <summary>
        /// Tests whether two specified vectors are equal.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the two vectors are equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(Vec3 u, Vec3 v) {
            return ValueType.Equals(u, v);
        }
        /// <summary>
        /// Tests whether two specified vectors are not equal.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the two vectors are not equal; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(Vec3 u, Vec3 v) {
            return !ValueType.Equals(u, v);
        }
        /// <summary>
        /// Tests if a vector's components are greater than another vector's components.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the left-hand vector's components are greater than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
        public static bool operator >(Vec3 u, Vec3 v) {
            return (
                (u.v1 > v.v1) &&
                (u.v2 > v.v2) &&
                (u.v3 > v.v3));
        }
        /// <summary>
        /// Tests if a vector's components are smaller than another vector's components.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the left-hand vector's components are smaller than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
        public static bool operator <(Vec3 u, Vec3 v) {
            return (
                (u.v1 < v.v1) &&
                (u.v2 < v.v2) &&
                (u.v3 < v.v3));
        }
        /// <summary>
        /// Tests if a vector's components are greater or equal than another vector's components.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the left-hand vector's components are greater or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
        public static bool operator >=(Vec3 u, Vec3 v) {
            return (
                (u.v1 >= v.v1) &&
                (u.v2 >= v.v2) &&
                (u.v3 >= v.v3));
        }
        /// <summary>
        /// Tests if a vector's components are smaller or equal than another vector's components.
        /// </summary>
        /// <param name="u">The left-hand vector.</param>
        /// <param name="v">The right-hand vector.</param>
        /// <returns><see langword="true"/> if the left-hand vector's components are smaller or equal than the right-hand vector's component; otherwise, <see langword="false"/>.</returns>
        public static bool operator <=(Vec3 u, Vec3 v) {
            return (
                (u.v1 <= v.v1) &&
                (u.v2 <= v.v2) &&
                (u.v3 <= v.v3));
        }
        #endregion

        #region Unary Operators
        /// <summary>
        /// Negates the values of the vector.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the negated values.</returns>
        public static Vec3 operator -(Vec3 v) {
            return new Vec3(-v.v1, -v.v2, -v.v3);
        }
        #endregion

        #region Binary Operators
        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="u">A <see cref="Vector3D"/> instance.</param>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
        public static Vec3 operator +(Vec3 u, Vec3 v) {
            return new Vec3(u.v1 + v.v1, u.v2 + v.v2, u.v3 + v.v3);
        }
        /// <summary>
        /// Adds a vector and a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
        public static Vec3 operator +(Vec3 v, double s) {
            return new Vec3(s + v.v1, s + v.v2, s + v.v3);
        }
        /// <summary>
        /// Adds a vector and a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the sum.</returns>
        public static Vec3 operator +(double s, Vec3 v) {
            return new Vec3(s + v.v1, s + v.v2, s + v.v3);
        }
        /// <summary>
        /// Subtracts a vector from a vector.
        /// </summary>
        /// <param name="u">A <see cref="Vector3D"/> instance.</param>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
        /// <remarks>
        ///	result[i] = v[i] - w[i].
        /// </remarks>
        public static Vec3 operator -(Vec3 u, Vec3 v) {
            return new Vec3(u.v1 - v.v1, u.v2 - v.v2, u.v3 - v.v3);
        }
        /// <summary>
        /// Subtracts a scalar from a vector.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
        /// <remarks>
        /// result[i] = v[i] - s
        /// </remarks>
        public static Vec3 operator -(Vec3 v, double s) {
            return new Vec3(v.v1 - s, v.v2 - s, v.v3 - s);
        }
        /// <summary>
        /// Subtracts a vector from a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> instance containing the difference.</returns>
        /// <remarks>
        /// result[i] = s - v[i]
        /// </remarks>
        public static Vec3 operator -(double s, Vec3 v) {
            return new Vec3(s - v.v1, s - v.v2, s - v.v3);
        }

        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> containing the result.</returns>
        public static Vec3 operator *(Vec3 v, double s) {
            return new Vec3(s * v.v1, s * v.v2, s * v.v3);
        }
        /// <summary>
        /// Multiplies a vector by a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar.</param>
        /// <returns>A new <see cref="Vector3D"/> containing the result.</returns>
        public static Vec3 operator *(double s, Vec3 v) {
            return new Vec3(s * v.v1, s * v.v2, s * v.v3);
        }

        public static Vec3 operator *(Vec3 v, Vec3 u) {
            return new Vec3(u.v1 * v.v1, u.v2 * v.v2, u.v3 * v.v3);
        }

        public static Vec3 operator /(Vec3 u, Vec3 v) {
            return new Vec3(u.v1 / v.v1, u.v2 / v.v2, u.v3 / v.v3);
        }

        /// <summary>
        /// Divides a vector by a scalar.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar</param>
        /// <returns>A new <see cref="Vector3D"/> containing the quotient.</returns>
        /// <remarks>
        /// result[i] = v[i] / s;
        /// </remarks>
        public static Vec3 operator /(Vec3 u, double s) {
            return new Vec3(u.v1 / s, u.v2 / s, u.v3 / s);
        }
        /// <summary>
        /// Divides a scalar by a vector.
        /// </summary>
        /// <param name="v">A <see cref="Vector3D"/> instance.</param>
        /// <param name="s">A scalar</param>
        /// <returns>A new <see cref="Vector3D"/> containing the quotient.</returns>
        /// <remarks>
        /// result[i] = s / v[i]
        /// </remarks>
        public static Vec3 operator /(double s, Vec3 v) {
            return new Vec3(s / v.v1, s / v.v2, s / v.v3);
        }
        #endregion

    }


}
