using UnityEngine;
using UnityEngine.UI;

public class ConnectionInfo : MonoBehaviour
{
	public InputField PortText;

	public InputField ServerIPText;

	private void Start()
	{
		if ((bool)UnitZ.gameServer)
		{
			if ((bool)PortText)
			{
				PortText.text = UnitZ.gameServer.Port.ToString();
			}
			if ((bool)ServerIPText)
			{
				ServerIPText.text = UnitZ.gameServer.IPServer;
			}
		}
	}

	public void SetServerIP(InputField num)
	{
		if ((bool)UnitZ.gameServer)
		{
			UnitZ.gameServer.IPServer = num.text;
		}
	}

	public void SetPort(InputField num)
	{
		if ((bool)UnitZ.gameServer)
		{
			int result = UnitZ.gameServer.Port;
			if (int.TryParse(num.text, out result))
			{
				UnitZ.gameServer.Port = result;
			}
		}
	}
}
