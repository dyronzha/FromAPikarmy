using UnityEngine;

namespace FromAPikarmy
{
	public class StaticObjectScroller : SceneObjectsScroller
	{
		[SerializeField] private Vector2 _spawnSizeRange;
		[SerializeField] private Sprite[] _spriteSamples;

		private int _randomLength;

		protected override void SetObject(SceneObject obj, Vector3 pos)
		{
			int id = Random.Range(0, _randomLength);
			var size =  Random.Range(_spawnSizeRange.x, _spawnSizeRange.y);
			var scale = new Vector3(size * RandomFlip(), size, 1);
			obj.SetSprite(_spriteSamples[id]);
			obj.SetTransformData(pos, size);
			obj.gameObject.SetActive(true);

			int RandomFlip()
			{
				return (Random.Range(0, 10) >= 5) ? 1 : -1;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_randomLength = _spriteSamples.Length;
		}
	}
}
