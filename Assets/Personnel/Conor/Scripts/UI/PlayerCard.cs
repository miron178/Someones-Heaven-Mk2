using UnityEngine;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] GameObject m_card;
    
    [SerializeField] TMP_Text m_positionText;
    public void SetPosition(int value) { m_positionText.text = $"{value}."; }

    [SerializeField] TMP_Text m_nameText;
    public void SetName(string value) { m_nameText.text = $"{value}"; }

    [SerializeField] TMP_Text m_timeText;
    public void SetTime(float value) { m_timeText.text = Utilities.ConvertFloatToTime(value); }
}
