public interface ISelectable
{
	void Select();

	void Select(int senderViewId);

	void Deselect();

	void Deselect(int senderViewId);
}
