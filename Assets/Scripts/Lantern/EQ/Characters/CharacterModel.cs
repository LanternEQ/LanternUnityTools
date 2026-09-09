using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Animation;
using Lantern.EQ.Equipment;
using Lantern.EQ.Lighting;
using UnityEngine;

namespace Lantern.EQ.Characters
{
    /// <summary>
    /// A directory class for all character models to avoid runtime component querying
    /// </summary>
    public class CharacterModel : MonoBehaviour
    {
        /// <summary>
        /// The renderers this character model uses
        /// </summary>
        public List<Renderer> Renderers;
        public SkeletonAttachPoints SkeletonAttachPoints;
        public Equipment2dHandler Equipment2dHandler;
        public Equipment3dHandler Equipment3dHandler;
        public AmbientLightSetterDynamic AmbientLightSetterDynamic;
        public CharacterAnimationLogic CharacterAnimationLogic;
        public CharacterSoundLogic CharacterSoundLogic;

        private void Awake()
        {
            if (CharacterAnimationLogic != null)
            {
                CharacterAnimationLogic.SetAnimationFiredCallback(OnAnimationPlayed);
            }
        }

        private void OnAnimationPlayed(AnimationType animationType)
        {
            // Propagate to animated equipment
            if (Equipment3dHandler != null)
            {
                Equipment3dHandler.PlayAnimation(animationType);
            }

            if (CharacterSoundLogic == null)
            {
                return;
            }

            // Sound is a combination of animation and equipment sound override
            if (AnimationHelper.IsAttackAnimation(animationType))
            {
                if (animationType == AnimationType.Combat1HSlashOffhand)
                {
                    //CharacterSoundLogic.PlaySound(EquipmentHelper.GetSoundForEquipment(Equipment3dHandler.EquipmentSoundPrimary));
                }
            }

            if (AnimationHelper.IsWalkRunInterrupt(animationType))
            {
                CharacterSoundLogic.InterruptWalkRunSound();
            }

            CharacterSoundLogic.PlaySound(AnimationHelper.GetSoundFromType(animationType));
        }

        public void SetLayer(int layer)
        {
            foreach (var r in Renderers)
            {
                r.gameObject.layer = layer;
            }

            if (Equipment3dHandler != null)
            {
                Equipment3dHandler.SetLayer(layer);
            }
        }

#if UNITY_EDITOR
        public void SetReferences(SkeletonAttachPoints skeletonAttachPoints, Equipment2dHandler equipment2dHandler,
            Equipment3dHandler equipment3dHandler, AmbientLightSetterDynamic
                ambientLightSetterDynamic, CharacterSoundLogic characterSounds,
            CharacterAnimationLogic characterAnimationLogic)
        {
            SkeletonAttachPoints = skeletonAttachPoints;
            Equipment2dHandler = equipment2dHandler;
            Equipment3dHandler = equipment3dHandler;
            AmbientLightSetterDynamic = ambientLightSetterDynamic;
            CharacterAnimationLogic = characterAnimationLogic;
            CharacterSoundLogic = characterSounds;
            Renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }
#endif
    }
}
