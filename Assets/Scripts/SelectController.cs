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
    [SerializeField]
    private Button leftArrow;
    [SerializeField]
    private Button rightArrow;

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
        if (page < 1)
            leftArrow.interactable = false;
        else
            leftArrow.interactable = true;
        //Right arrow limit is arbitrary, just a temporary wall so infinite pages aren't available
        if (page > 20)
            rightArrow.interactable = false;
        else
            rightArrow.interactable = true;
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
            case 6:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Wizard&Pickpocket";
                }
                break;
            case 7:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Wizard+Pickpocket";
                }
                break;
            case 8:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Sailor";
                }
                break;
            case 9:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Sailor";
                }
                break;
            case 10:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Wizard & Pickpocket & Sailor";
                }
                break;
            case 11:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Wizard & Pickpocket & Sailor";
                }
                break;
            default:
                for (int i = 0; i < levelBtns.Count; i++)
                {
                    Button btn = levelBtns[i];
                    GameObject childText = btn.transform.GetChild(0).gameObject;
                    TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
                    btnText.color = new Color(0.03f, 0.43f, 0.06f, 1);
                    btnText.text = $"{i + 1 + (page * 15)}";
                    charText.text = "Full Party";
                }
                break;
        }
        foreach(Button btn in levelBtns)
        {
            GameObject childText = btn.transform.GetChild(0).gameObject;
            TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
            if (!GameData.ContainsLevel(btnText.text))
            {
                btn.interactable = false;
            }
            else
                btn.interactable = true;
        }
    }

    public void LevelSelected(Button button)
    {
        GameObject childText = button.transform.GetChild(0).gameObject;
        TextMeshProUGUI btnText = childText.GetComponent<TextMeshProUGUI>();
        string levelNum = btnText.text;
        SceneManager.LoadScene(levelNum);
    }

    public void PageLeft()
    {
        if (page > 0)
            page--;
        UpdatePage();
    }

    public void PageRight()
    {
        page++;
        UpdatePage();
    }
}
