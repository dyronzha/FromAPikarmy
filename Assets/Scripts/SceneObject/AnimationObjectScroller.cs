using UnityEngine;


namespace FromAPikarmy
{
	public class AnimationObjectScroller : SceneObjectsScroller
	{
		[SerializeField] private RuntimeAnimatorController _animator;

		private int[] _animationID;

		protected override void SetObject(SceneObject obj, Vector3 pos)
		{
			obj.gameObject.SetActive(true);
			int id = Random.Range(0, _animationID.Length);
			obj.SetAnimation(_animationID[id]);
			obj.SetPosition(pos);
			
		}

		protected override void Awake()
		{
			base.Awake();
			var allClips = _animator.animationClips;
			_animationID = new int[allClips.Length];
			int i = 0;
			foreach (AnimationClip clip in allClips)
			{
				_animationID[i] = Animator.StringToHash(clip.name);
				i++;
			}
		}
	}
}
