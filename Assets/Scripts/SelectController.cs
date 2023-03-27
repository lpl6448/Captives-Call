using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectController : MonoBehaviour
{
    [SerializeField]
    private List<Button> levelBtns;
    [SerializeField]
    private TextMeshProUGUI charText;

    private Button selectedButton;

    private int page;

    // Start is called before the first frame update
    void Start()
    {
        page = 0;
        UpdatePage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePage()
    {
        switch(page)
        {
            case 0:
                for(int i = 0; i<levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.93f, 0.29f, 0.19f, 1);
                    btnText.text = $"{i+1+(page*15)}";
                    charText.text = "Warlock";
                }
                break;
            case 1:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.93f, 0.29f, 0.19f, 1);
                    btnText.text = $"{i+1+(page*15)}";
                    charText.text = "Warlock";
                }
                break;
            case 2:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.20f, 0.18f, 0.93f, 1);
                    btnText.text = $"{i + 1 + (page*15)}";
                    charText.text = "Wizard";
                }
                break;
            case 3:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.20f, 0.18f, 0.93f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Wizard";
                }
                break;
            case 4:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.26f, 0.03f, 0.43f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Pickpocket";
                }
                break;
            case 5:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.26f, 0.03f, 0.43f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Pickpocket";
                }
                break;
            //case 6:
            //    for (int i = 0; i < levelBtns.Count; i++)
            //    {
            //        Button btn = levelBtns[i];
            //        GameObject childText = btn.transform.GetChild(0).gameObject;
            //        TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
            //        btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
            //        btnText.text = $"{i + 1 + (page * 15)}";
            //        charText.text = "Sailor";
            //    }
            //    break;
        }
    }

    public void LevelSelected(Button button)
    {

    }
}
