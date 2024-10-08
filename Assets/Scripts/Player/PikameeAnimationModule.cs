using UnityEngine;

namespace FromAPikarmy
{
	[RequireComponent(typeof(Animator))]
	public class PikameeAnimationModule : MonoBehaviour
	{
		[SerializeField] private string _eatName;
		[SerializeField] private string _eatParameter;
        [SerializeField] private string _kettleReadyParameter;
        [SerializeField] private string _kettleLaughParameter;
        [SerializeField] private string _kettleLaughEndParameter;
        [SerializeField] private Animator _animator;
		[SerializeField] private RuntimeAnimatorController[] _skinAni;

		private bool _eatingDonut;
		private int _eatNameHash;
		private int _eatParameterHash;
        private int _kettleReadyParameterHash;
        private int _kettleLaughParameterHash;
        private int _kettleLaughEndParameterHash;
        private float _eatingTimer;

		public void PlayEat()
		{
			_eatingDonut = true;
			_eatingTimer = 0f;
			float normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			normalizedTime = normalizedTime - (int)normalizedTime;
			_animator.Play(_eatNameHash, 1, normalizedTime);
			_animator.SetBool(_eatParameterHash, true);
		}

		public void PlayKettleReady()
		{
            _animator.SetTrigger(_kettleReadyParameterHash);
        }

        public void PlayKettleLaugh()
        {
            _animator.SetTrigger(_kettleLaughParameterHash);
        }

		public void EndKettleLaugh()
		{
            _animator.SetTrigger(_kettleLaughEndParameterHash);
        }

        private void Awake()
		{
			_animator.runtimeAnimatorController = _skinAni[(int)SkinManager.Instance.Skin];
			_eatNameHash = Animator.StringToHash(_eatName);
			_eatParameterHash = Animator.StringToHash(_eatParameter);
            _kettleReadyParameterHash = Animator.StringToHash(_kettleReadyParameter);
            _kettleLaughParameterHash = Animator.StringToHash(_kettleLaughParameter);
            _kettleLaughEndParameterHash = Animator.StringToHash(_kettleLaughEndParameter);
        }

		private void LateUpdate()
		{
			if (_eatingDonut)
			{
				_eatingTimer += Time.deltaTime;
				if (_eatingTimer >= 1.0f)
				{
					EndEat();
				}
			}
		}

		private void EndEat()
		{
			_eatingDonut = false;
			_eatingTimer = 0f;
			_animator.SetBool(_eatParameterHash, false);
		}
	}
}
