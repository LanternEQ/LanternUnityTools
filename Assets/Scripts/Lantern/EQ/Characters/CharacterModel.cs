using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Animation;
using Lantern.EQ.Equipment;
using Lantern.EQ.Lighting;
using Lantern.EQ.Sound;
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
        public CharacterSoundsBase CharacterSounds;
        public CharacterAnimationLogic CharacterAnimationLogic;

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
                ambientLightSetterDynamic, CharacterSoundsBase characterSounds,
            CharacterAnimationLogic characterAnimationLogic)
        {
            SkeletonAttachPoints = skeletonAttachPoints;
            Equipment2dHandler = equipment2dHandler;
            Equipment3dHandler = equipment3dHandler;
            AmbientLightSetterDynamic = ambientLightSetterDynamic;
            CharacterSounds = characterSounds;
            CharacterAnimationLogic = characterAnimationLogic;
            Renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }
#endif
    }
}
