using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MiracleGun.Invariants
{
    public class GunPowder {
        #region Поля
        /// <summary>
        /// Сила пороха [MДж/кг]
        /// </summary>
        public double f;

        /// <summary>
        /// Показатель в законе горения
        /// </summary>
        public double nu;

        /// <summary>
        /// Коэффициент в законе горения
        /// </summary>
        public double u1;

        /// <summary>
        /// Импульс конца горения пороха [МПа*с]
        /// </summary>
        public double Ik; 

        /// <summary>
        /// Коволюм [дм3/кг]
        /// </summary>
        public double alpha_k;
        
        /// <summary>
        /// Показатель адиабаты пороховых газов
        /// </summary>
        public double k;

        /// <summary>
        /// Массовая плотность пороха [кг/дм3]
        /// </summary>
        public double dest;

        /// <summary>
        /// Температура горения пороха [К]
        /// </summary>
        public double T1;

        /// <summary>
        /// Относительная толщина пороха в конце горения
        /// </summary>
        public double zk;

        /// <summary>
        /// Есть в экселе, но хз что это
        /// </summary>
        public double kappa_f, kappa_l;

        public double kappa_1, kappa_2, lambda_1, lambda_2;            // коэффициенты формы порохового заряда

        #endregion
        
        #region Функции пси и закон горения
        /// <summary>
        /// Функция газоприхода (относительный объем сгоревшего свода)
        /// </summary>
        /// <param name="z"> Относительная толщина сгоревшего слоя </param>
        public double Psi(double z) {
            if (z < 0) {
                return 0;
            } 
            if (z < 1) {
                return kappa_1 * z * (1 + 2 * lambda_1 * z);
            }
            else if (z >= 1 & z < zk) {
                return kappa_1 * (1 + lambda_1) + kappa_2 * (z - 1) * (1 + lambda_2 * (z - 1));
            }
            else {
                return 1;
            }
        }

        /// <summary>
        /// Производная psi по z
        /// </summary>
        /// <param name="z"></param>
        /// <returns></returns>
        public double Dpsi_dz(double z) {
            if (z < 1) {
                return kappa_1 * (1 + 2 * lambda_1 * z);
            }
            else if (z >= 1 & z < zk) {
                return kappa_2 * (1 + 2 * lambda_2 * (z - 1));
            }
            else if (z < 0) {
                return 0;
            }
            else {
                return 0;
            }
        }

        /// <summary>
        /// Скорость горения пороха
        /// </summary>
        /// <param name="p"> Давление </param>
        public virtual double U(double p) {
            return u1 * Math.Pow(p, nu);
        }
        #endregion

        public static GunPowder Factory(string powderName) {
            return GunPowderFactory.Get(powderName);
        }

    }
}
