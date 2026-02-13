using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoonzuGame
{
    [System.Serializable]
    public class SpriteAnimation
    {
        public string animationName;
        public List<Sprite> frames = new List<Sprite>();
        public float frameRate = 12f;
        public bool loop = true;
        public bool pingPong = false;

        [HideInInspector]
        public float duration { get { return frames.Count / frameRate; } }
    }

    public class GoonzuSpriteAnimator : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Dictionary<string, SpriteAnimation> animations = new Dictionary<string, SpriteAnimation>();
        private SpriteAnimation currentAnimation;
        private int currentFrame;
        private float timer;
        private bool isPlaying;
        private bool reverse;
        private System.Action onAnimationComplete;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
        }

        void Update()
        {
            if (!isPlaying || currentAnimation == null) return;

            timer += Time.deltaTime;

            if (timer >= (1f / currentAnimation.frameRate))
            {
                timer = 0f;
                UpdateFrame();
            }
        }

        void UpdateFrame()
        {
            if (currentAnimation.frames.Count == 0) return;

            if (currentAnimation.pingPong)
            {
                if (reverse)
                {
                    currentFrame--;
                    if (currentFrame < 0)
                    {
                        currentFrame = 1;
                        reverse = false;
                        if (!currentAnimation.loop)
                        {
                            StopAnimation();
                            return;
                        }
                    }
                }
                else
                {
                    currentFrame++;
                    if (currentFrame >= currentAnimation.frames.Count)
                    {
                        currentFrame = currentAnimation.frames.Count - 2;
                        reverse = true;
                        if (!currentAnimation.loop)
                        {
                            StopAnimation();
                            return;
                        }
                    }
                }
            }
            else
            {
                currentFrame++;
                if (currentFrame >= currentAnimation.frames.Count)
                {
                    currentFrame = 0;
                    if (!currentAnimation.loop)
                    {
                        StopAnimation();
                        return;
                    }
                }
            }

            spriteRenderer.sprite = currentAnimation.frames[currentFrame];
        }

        // Public API methods
        public void AddAnimation(string name, List<Sprite> frames, float frameRate = 12f, bool loop = true, bool pingPong = false)
        {
            SpriteAnimation anim = new SpriteAnimation
            {
                animationName = name,
                frames = new List<Sprite>(frames),
                frameRate = frameRate,
                loop = loop,
                pingPong = pingPong
            };
            animations[name] = anim;
        }

        public void AddAnimation(string name, Sprite[] frames, float frameRate = 12f, bool loop = true, bool pingPong = false)
        {
            AddAnimation(name, new List<Sprite>(frames), frameRate, loop, pingPong);
        }

        public void PlayAnimation(string animationName, System.Action onComplete = null)
        {
            if (!animations.ContainsKey(animationName))
            {
                Debug.LogWarning($"Animation '{animationName}' not found");
                return;
            }

            currentAnimation = animations[animationName];
            currentFrame = 0;
            timer = 0f;
            reverse = false;
            isPlaying = true;
            onAnimationComplete = onComplete;

            if (currentAnimation.frames.Count > 0)
            {
                spriteRenderer.sprite = currentAnimation.frames[0];
            }
        }

        public void StopAnimation()
        {
            isPlaying = false;
            currentAnimation = null;
            onAnimationComplete?.Invoke();
            onAnimationComplete = null;
        }

        public void PauseAnimation()
        {
            isPlaying = false;
        }

        public void ResumeAnimation()
        {
            if (currentAnimation != null)
            {
                isPlaying = true;
            }
        }

        public bool IsPlaying(string animationName = null)
        {
            if (animationName == null)
                return isPlaying;

            return isPlaying && currentAnimation != null && currentAnimation.animationName == animationName;
        }

        public string GetCurrentAnimationName()
        {
            return currentAnimation?.animationName;
        }

        public float GetAnimationProgress()
        {
            if (currentAnimation == null || currentAnimation.frames.Count == 0) return 0f;
            return (float)currentFrame / currentAnimation.frames.Count;
        }

        public void SetFrameRate(float frameRate)
        {
            if (currentAnimation != null)
            {
                currentAnimation.frameRate = frameRate;
            }
        }

        public void LoadCharacterAnimations(string characterName)
        {
            // Load character-specific animations
            // This assumes sprite sheets are named with patterns like:
            // characterName_idle_0, characterName_idle_1, etc.

            LoadAnimationSet(characterName, "idle", 4, 8f, true);
            LoadAnimationSet(characterName, "walk", 6, 12f, true);
            LoadAnimationSet(characterName, "run", 6, 16f, true);
            LoadAnimationSet(characterName, "attack", 4, 12f, false);
            LoadAnimationSet(characterName, "cast", 3, 8f, false);
            LoadAnimationSet(characterName, "hurt", 2, 8f, false);
            LoadAnimationSet(characterName, "death", 5, 8f, false);
        }

        void LoadAnimationSet(string characterName, string animationType, int frameCount, float frameRate, bool loop)
        {
            List<Sprite> frames = new List<Sprite>();

            for (int i = 0; i < frameCount; i++)
            {
                string spriteName = $"{characterName}_{animationType}_{i}";
                Sprite sprite = GoonzuAssetManager.Instance.GetSprite(spriteName);
                if (sprite != null)
                {
                    frames.Add(sprite);
                }
            }

            if (frames.Count > 0)
            {
                AddAnimation($"{characterName}_{animationType}", frames, frameRate, loop);
            }
        }

        public void LoadEffectAnimation(string effectName)
        {
            // Load effect animations (typically short, non-looping)
            List<Sprite> frames = new List<Sprite>();
            int frameIndex = 0;

            while (true)
            {
                string spriteName = $"{effectName}_{frameIndex}";
                Sprite sprite = GoonzuAssetManager.Instance.GetSprite(spriteName);
                if (sprite == null) break;

                frames.Add(sprite);
                frameIndex++;
            }

            if (frames.Count > 0)
            {
                AddAnimation(effectName, frames, 15f, false); // Effects typically play once at 15fps
            }
        }
    }
}