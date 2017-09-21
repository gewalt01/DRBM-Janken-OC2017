using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using drbm_c_sharp;

public class click001 : MonoBehaviour
{
    public GameObject playerResult;
    public Image playerResultImage;
    public GameObject comResult;
    public Image comResultImage;
    public UnityJanken janken;
    public int count = 0;
    public string uid = "0";
    public int[] resultCount = new int[3] { 0, 0, 0 };
    public double[] resultRate = new double[3] { 0.0, 0.0, 0.0 };
    public string jsonParams;  // FIXME: 削除予定
    protected bool flagShowComNextHand = false;

    void init()
    {
        count = 0;
        resultCount = new int[3] { 0, 0, 0 };
        resultRate = new double[3] { 0.0, 0.0, 0.0 };
        janken = new UnityJanken(5);
        showResult();

        // 学習回数更新
        showTrainCount();

        // to unvisible crown
        GameObject.FindWithTag("PlayerWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        GameObject.FindWithTag("ComWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        // unvisible history
        var histObj = GameObject.FindWithTag("History");
        var children = histObj.GetComponentsInChildren<Image>();
        for (int i = 0; i < children.Length; i++)
        {
            var texture = new Texture2D(1, 1);
            children[i].sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        // unvisivle result message
        GameObject.FindWithTag("msg").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);


        // next com hand
        showComNextHand();

    }

    // Use this for initialization
    void Start()
    {
        //DontDestroyOnLoad(this);
        init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // グー
    public void JankenRock()
    {
        int my_no = 0;
        _game(my_no);
    }

    // チョキ
    public void JankenScissors()
    {
        int my_no = 1;
        _game(my_no);
    }

    // パー
    public void JankenPaper()
    {
        int my_no = 2;
        _game(my_no);

    }

    protected void _game(int player_hand_no)
    {
        string[] janken_res = new string[] { "janken001", "janken002", "janken003" };
        string[] janken_com_res = new string[] { "janken004", "janken005", "janken006" };

        // game
        int com_hand_no = janken.inference();
        janken.game(player_hand_no);

        // show result

        int judge = janken.judge(player_hand_no, com_hand_no);
        count++;
        resultCount[judge]++;
        resultRate[0] = resultCount[0];
        resultRate[1] = resultCount[1];
        resultRate[2] = resultCount[2];

        // player
        playerResult = GameObject.FindWithTag("PlayerHand");
        playerResult.SetActive(false);
        playerResultImage = playerResult.GetComponent<Image>();
        Texture2D texturePlayer = Resources.Load(janken_res[player_hand_no]) as Texture2D;
        playerResultImage.sprite = Sprite.Create(texturePlayer, new Rect(0, 0, texturePlayer.width, texturePlayer.height), Vector2.zero);
        playerResult.SetActive(true);

        // com
        comResult = GameObject.FindWithTag("ComHand");
        comResult.SetActive(false);
        comResultImage = comResult.GetComponent<Image>();
        Texture2D textureCom = Resources.Load(janken_com_res[com_hand_no]) as Texture2D;
        comResultImage.sprite = Sprite.Create(textureCom, new Rect(0, 0, textureCom.width, textureCom.height), Vector2.zero);
        comResult.SetActive(true);

        showResult();
        showResultMessage(judge);
        showCrown(judge);
        showHistory();
        showComNextHand();

        // 学習回数更新
        showTrainCount();

        Debug.Log(count);
    }

    void showResult()
    {
        GameObject.FindWithTag("count").GetComponent<Text>().text = "Try: " + count.ToString();
        GameObject.FindWithTag("win").GetComponent<Text>().text = "Player Win: " + resultRate[0].ToString();
        GameObject.FindWithTag("draw").GetComponent<Text>().text = "Draw: " + resultRate[1].ToString();
        GameObject.FindWithTag("lose").GetComponent<Text>().text = "Com Win: " + resultRate[2].ToString();
        //Debug.Log(JsonUtility.ToJson(this.janken.drbm.param));
    }

    public void reset()
    {
        init();
    }

    public void changeToSetati()
    {
        SceneManager.LoadScene("janken_settai");
    }

    /*
    public void saveClicked()
    {
        uid = GameObject.FindWithTag("uid").GetComponent<Text>().text;
        StartCoroutine(jsonPost());
    }
    */

    /*
    public IEnumerator jsonPost()
    {
        // パラメータJSON化
        jsonParams = JsonUtility.ToJson(this.janken.drbm.param);
        // HTTP通信
        // TODO: 通信をJSON-RPC仕様にしたい

        var url = "http://localhost:3000/users/connection/" + uid;
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonParams);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);
    }
    */

    /*
    public void loadClicked()
    {
        uid = GameObject.FindWithTag("uid").GetComponent<Text>().text;
        StartCoroutine(jsonGet());
    }
    */

    /*
    public IEnumerator jsonGet()
    {
        // HTTP通信
        // TODO: 通信をJSON-RPC仕様にしたい

        var url = "http://localhost:3000/users/connection/" + uid;
        var request = new UnityWebRequest(url, "GET");
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.Send();

        Debug.Log("Status Code: " + request.responseCode);

        // JSONデータ, DRBMパラメータ化
        if (request.responseCode == 200)
        {
            Debug.Log("success");
            string text = request.downloadHandler.text;
            jsonParams = text;
            this.janken.drbm.param = JsonUtility.FromJson<DRBMParamator>(jsonParams);

            // 学習回数更新
            showTrainCount();
        }
    }
    */

    public void showTrainCount()
    {
        GameObject.FindWithTag("train_count").GetComponent<Text>().text = janken.drbm.param.trainCount.ToString();
    }

    public void showCrown(int result_no)
    {
        // to unvisible crown
        GameObject.FindWithTag("PlayerWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        GameObject.FindWithTag("ComWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);


        switch (result_no)
        {
            default:
                break;
            case 0: // Player Win
                GameObject.FindWithTag("PlayerWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
            case 1: // Draw
                break;
            case 2: // Com Win
                GameObject.FindWithTag("ComWin").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                break;
        }
    }

    public void showHistory()
    {
        string[] janken_res = new string[] { "janken001", "janken002", "janken003" };

        var histObj = GameObject.FindWithTag("History");
        var children = histObj.GetComponentsInChildren<Image>();
        var history = janken.history.ToArray();
        for (int i = 0; i < children.Length; i++)
        {
            if (history.Length <= i) break;  // if out of range of history

            //history = [0, 1, 2, 3, 4]; // old->new
            var hand_no = history[history.Length - 1 - i];

            Texture2D textureCom = Resources.Load(janken_res[hand_no]) as Texture2D;
            children[children.Length - 1 - i].sprite = Sprite.Create(textureCom, new Rect(0, 0, textureCom.width, textureCom.height), Vector2.zero);
        }
    }

    public void showComNextHand()
    {
        string[] janken_com_res = new string[] { "janken004", "janken005", "janken006" };
        int com_hand_no = janken.inference();


        var comNext = GameObject.FindWithTag("ComNext");
        var comNextImage = comNext.GetComponent<Image>();
        if (flagShowComNextHand == true)
        {
            Texture2D textureCom = Resources.Load(janken_com_res[com_hand_no]) as Texture2D;
            comNextImage.sprite = Sprite.Create(textureCom, new Rect(0, 0, textureCom.width, textureCom.height), Vector2.zero);
        }
        else
        {
            Texture2D textureCom = Resources.Load("question") as Texture2D;
            comNextImage.sprite = Sprite.Create(textureCom, new Rect(0, 0, textureCom.width, textureCom.height), Vector2.zero);
        }
    }

    public void flipComNextFlag()
    {
        flagShowComNextHand = !flagShowComNextHand;
        var buttonObj = GameObject.FindWithTag("DisplayComButton");
        var children = buttonObj.GetComponentsInChildren<Text>();
        children[0].text = flagShowComNextHand ? "Undisplay COM's Next??" : "Display COM's Next???";
        showComNextHand();
    }

    public void showResultMessage(int result_no)
    {
        string[] msg = new string[] { "honki_win", "honki_draw", "honki_lose" };


        Texture2D textureCom = Resources.Load(msg[result_no]) as Texture2D;
        GameObject.FindWithTag("msg").GetComponent<Image>().sprite = Sprite.Create(textureCom, new Rect(0, 0, textureCom.width, textureCom.height), Vector2.zero);


        // to visible result message
        GameObject.FindWithTag("msg").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);


    }
}
