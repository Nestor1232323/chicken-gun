using UnityEngine;

namespace Photon
{
	public class MonoBehaviour : UnityEngine.MonoBehaviour
	{
		private PhotonView pvCache;

		public PhotonView photonView
		{
			get
			{
				if (pvCache == null)
				{
					pvCache = PhotonView.Get(this);
				}
				return pvCache;
			}
		}
	}
}
