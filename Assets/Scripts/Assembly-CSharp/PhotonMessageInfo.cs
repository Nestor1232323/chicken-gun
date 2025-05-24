public struct PhotonMessageInfo
{
	private readonly int timeInt;

	public readonly PhotonPlayer sender;

	public readonly PhotonView photonView;

	public double timestamp => (double)(uint)timeInt / 1000.0;

	public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
	{
		sender = player;
		timeInt = timestamp;
		photonView = view;
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", timestamp, sender);
	}
}
