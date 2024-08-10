using UnityEngine;

namespace FromAPikarmy
{
	public class SpecialMoveModule : MonoBehaviour
	{
		[SerializeField]
		private int _maxEnergy;
		[SerializeField]
		private float _inSpecialTime;
		[SerializeField]
		Transform _energyBarFillTransform;
		[SerializeField]
		PikameeAnimationModule _animationModule;
		[SerializeField]
		private DonutManager _pikarmyManager;

		private bool _isLock = true;
		private int _curEnergy;
		private float _timer;

		public bool InSpecial { get; private set; } = false;

        public void AddEnergy(Player.EatData eatData)
		{
			if (_isLock && !InSpecial)
			{
                _curEnergy += eatData.Count;
				float ratio = Mathf.Clamp01((float)_curEnergy / _maxEnergy);
                _energyBarFillTransform.localScale = new Vector3(ratio, 1, 1);
                if (_curEnergy >= _maxEnergy)
                {
                    Unlock();
                    _curEnergy = _maxEnergy;
                }
            }
		}

		public void TriggerOn()
		{
			if (_isLock || InSpecial)
			{
				return;
			}
			InSpecial = true;
			_curEnergy = 0;
            _animationModule.PlayKettleLaugh();
			_pikarmyManager.enabled = true;
        }

		private void Update()
		{
			if (InSpecial)
            {
				_timer += Time.deltaTime;
				if (_timer >= _inSpecialTime)
				{
                    Lock();
                    _timer = 0;
				}
				else
				{
					float ratio = Mathf.Clamp01((_inSpecialTime - _timer) / _inSpecialTime);
                    _energyBarFillTransform.localScale = new Vector3(ratio, 1, 1);
                }
			}
		}

		private void Unlock()
		{
			_isLock = false;
			_animationModule.PlayKettleReady();
        }

		private void Lock()
		{
			InSpecial = false;
			_isLock = true;
			_energyBarFillTransform.localScale = new Vector3(0, 1, 1);
            _animationModule.EndKettleLaugh();
			_pikarmyManager.enabled = false;
        }
	}
}
