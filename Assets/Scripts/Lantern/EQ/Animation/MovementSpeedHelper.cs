using Lantern.EQ.Data;
using Lantern.EQ.Lantern;

namespace Lantern.EQ.Animation
{
   public static class MovementSpeedHelper
   {
      public static float GetBaseWalkSpeedInLanternUnits()
      {
         return GetEqToLanternSpeed(EqConstants.SpeedWalk);
      }

      public static float GetBaseRunSpeedInLanternUnits()
      {
         return GetEqToLanternSpeed(EqConstants.SpeedRun);
      }

      public static float GetEqToLanternSpeed(float speed)
      {
         return EqConstants.Velocity * speed * LanternConstants.WorldScale;
      }
   }
}
