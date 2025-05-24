public class RaiseEventOptions
{
	public static readonly RaiseEventOptions Default = new RaiseEventOptions();

	public EventCaching CachingOption;

	public byte InterestGroup;

	public int[] TargetActors;

	public ReceiverGroup Receivers;

	public byte SequenceChannel;

	public bool ForwardToWebhook;

	public bool Encrypt;

	public void Reset()
	{
		CachingOption = Default.CachingOption;
		InterestGroup = Default.InterestGroup;
		TargetActors = Default.TargetActors;
		Receivers = Default.Receivers;
		SequenceChannel = Default.SequenceChannel;
		ForwardToWebhook = Default.ForwardToWebhook;
		Encrypt = Default.Encrypt;
	}
}
