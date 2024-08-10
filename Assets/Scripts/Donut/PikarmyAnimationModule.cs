using UnityEngine;

namespace FromAPikarmy
{
	public class PikarmyAnimationModule : MonoBehaviour
	{
		[SerializeField] private string[] _runName;
		[SerializeField] private string[] _eatenName;
		[SerializeField] private string[] _accessoryName;

		private int _index = 0;
		private int[] _runNameHashID;
		private int[] _eatenNameHashID;
        private int[] _accessoryNameHashID;
        private Animator _animator;
        private Animator Animator
        {
            get
            {
                if (_animator == null)
                {
                    _animator = GetComponent<Animator>();
                }
                return _animator;
            }
        }

        public void Init()
        {
            int length = _runName.Length;
            _index = Random.Range(0, length);
            _runNameHashID = new int[length];
            for (int i = 0; i < length; i++)
            {
                _runNameHashID[i] = Animator.StringToHash(_runName[i]);
            }
            length = _eatenName.Length;
            _eatenNameHashID = new int[length];
            for (int i = 0; i < length; i++)
            {
                _eatenNameHashID[i] = Animator.StringToHash(_eatenName[i]);
            }
            length = _accessoryName.Length;
            _accessoryNameHashID = new int[length];
            for (int i = 0; i < length; i++)
            {
                _accessoryNameHashID[i] = Animator.StringToHash(_accessoryName[i]);
            }
        }
        public void PlayRun()
		{
            var animator = Animator;
            var id = _runName[_index];
			animator.Play(id, 0);
            var length = _accessoryNameHashID.Length;
            bool hasAccessory = false;
            int index = Random.Range(0, length);
            for (int count = 0; count < length; count++)
            {
                index %= length;
                if (Random.Range(0, 10) >= 5)
                {
                    hasAccessory = true;
                    animator.SetBool(_accessoryNameHashID[index], true);
                }
                else if (count == length - 1 && !hasAccessory)
                {
                    animator.SetBool(_accessoryNameHashID[index], true);
                }
                index++;
            }
        }

		public void PlayEaten()
		{
			var id = _eatenName[_index];
            Animator.Play(id, 0);
            foreach(var accrssoryID in _accessoryNameHashID)
            {
                Animator.SetBool(accrssoryID, false);
            }
        }
	}
}
