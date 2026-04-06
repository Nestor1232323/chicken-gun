using System.Collections;

public class PlaneCarController : CarController
{
	public RotateObjest rotor;

	protected override IEnumerator Start()
	{
		rotor.Stop();
		return base.Start();
	}

	[PunRPC]
	public override void OnPlayerSitMeRPC(int playerViewId)
	{
		base.OnPlayerSitMeRPC(playerViewId);
		rotor.StartRotating();
	}

	[PunRPC]
	public override IEnumerator OnPlayerLeaveMeRPC()
	{
		rotor.Stop();
		return base.OnPlayerLeaveMeRPC();
	}
}
