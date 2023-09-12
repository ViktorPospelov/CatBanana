using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YG;
using Random = UnityEngine.Random;

public class Pole : MonoBehaviour
{
    [SerializeField] private GameObject block;
    [SerializeField] private GameObject winObj;
    [SerializeField] private TextMeshProUGUI cheiHod;
    [SerializeField] private TextMeshProUGUI winer;
    [SerializeField] private Transform brick;
    [SerializeField] private cat _cat;

    private List<Transform> bricks;

    private hod Hod;

    private bool _onBot;

    public static Action ClikMove;

    private static bool LangRu = true;

    private void Start()
    {
        NormalStart();
        
        int saveGame = _onBot ? 2:1;
        int saveLeght = PlayerPrefs.GetInt($"{saveGame}SaveLeght", 0);
        if (saveLeght != 0)
        {
            SetBricks();
            foreach (var br in bricks)
            {
                br.transform.position = new Vector3(1000, 100, 0);
            }
            for (int i = 0; i < saveLeght; i++)
            {
                var br = bricks[i];
               br.position = new Vector3(PlayerPrefs.GetFloat($"{saveGame}posX{i}"),PlayerPrefs.GetFloat($"{saveGame}posY{i}"));
               br.rotation = Quaternion.Euler(0,0,PlayerPrefs.GetFloat($"{saveGame}rotationZ{i}"));
            }
            _cat.gameObject.transform.position = new Vector3(PlayerPrefs.GetFloat($"{saveGame}posXCat"),PlayerPrefs.GetFloat($"{saveGame}posYCat"));
            _cat.gameObject.transform.rotation = Quaternion.Euler(0,0,PlayerPrefs.GetFloat($"{saveGame}rotationZCat"));

        }
    }
    

    private void NormalStart()
    {
        Hod = hod.player1;
        ClikMove += SetClikMove;
        YandexGame.LanguageRequest();
        LangRu = YandexGame.EnvironmentData.language == "ru";
        cheiHod.text = LangRu ? "Игрок 1" : "Player 1";
        cheiHod.color = Color.blue;
        GameControler.EndGame += NewGame;
    }

    private void NewGame()
    {
        block.SetActive(true);
        winObj.SetActive(true);
        int saveGame = _onBot ? 2:1;
        for (int i = 0; i < 100; i++)
        {
            PlayerPrefs.DeleteKey($"{saveGame}posX{i}");
            PlayerPrefs.DeleteKey($"{saveGame}posY{i}");
            PlayerPrefs.DeleteKey($"{saveGame}rotationZ{i}");
        }
        PlayerPrefs.SetInt($"{saveGame}SaveLeght", 0);
        
        PlayerPrefs.DeleteKey($"{saveGame}posXCat");
        PlayerPrefs.DeleteKey($"{saveGame}posYCat");
        PlayerPrefs.DeleteKey($"{saveGame}rotationZCat");

    }

    private void SetClikMove()
    {
        block.SetActive(true);
        StartCoroutine(OffBlock());

        //если бот
    }

    private BrickControl GetBrick()
    {
        int i = Random.Range(0, bricks.Count);
        return bricks[i].gameObject.GetComponent<BrickControl>();
    }

    private void SetBricks()
    {
        bricks = new List<Transform>();
        foreach (Transform b in brick.transform)
        {
            bricks.Add(b);
        }
    }

    public void SetBot(bool b)
    {
        _onBot = b;
    }

    public void SetBlock(bool bl)
    {
        block.SetActive(bl);
    }

    private IEnumerator OffBlock()
    {
        if (!winObj.activeSelf)
        {
            SProgress();
            cheiHod.text = LangRu ? "ждите" : "wait";
            cheiHod.color = Color.black;
            yield return new WaitForSeconds(1.5f);
            if (Hod == hod.player1) Hod = _onBot ? hod.bot : hod.player2;
            else if (Hod == hod.player2 || Hod == hod.bot) Hod = hod.player1;
            switch (Hod)
            {
                case hod.player1:
                    cheiHod.text = LangRu ? "Игрок 1" : "Player 1";
                    cheiHod.color = Color.blue;
                    winer.color = Color.blue;
                    break;
                case hod.player2:
                    cheiHod.text = LangRu ? "Игрок 2" : "Player 2";
                    cheiHod.color = Color.red;
                    winer.color = Color.red;
                    break;
                case hod.bot:
                    cheiHod.text = LangRu ? "Компьютер" : "Computer";
                    cheiHod.color = Color.magenta;
                    winer.color = Color.magenta;
                    if (_onBot)
                    {
                        SetBricks();
                        StartCoroutine(BotMove());
                    }
                    break;
            }

            winer.text = LangRu ? "Победитель:" : "Winner:";
            if (Hod != hod.bot) block.SetActive(false);
        }
    }

    private IEnumerator BotMove()
    {
        block.SetActive(true);
        yield return new WaitForSeconds(Random.Range(70, 150) * 0.01f);
        if(!winObj.activeSelf)GetBrick().Click();
    }

    private void OnDestroy()
    {
        ClikMove -= SetClikMove;
        GameControler.EndGame -= NewGame;
    }

    private void SProgress()
    {
        SetBricks();
        var type = _onBot ? 2:1;
        PlayerPrefs.SetInt($"{type}SaveLeght", bricks.Count);

        for (int i = 0; i < bricks.Count; i++)
        {
            SaveProgressBrick(i, bricks[i].gameObject.transform.position.x, bricks[i].gameObject.transform.position.y, bricks[i].gameObject.transform.rotation.z);
        }
        PlayerPrefs.SetFloat($"{type}posXCat",_cat.gameObject.transform.position.x);
        PlayerPrefs.SetFloat($"{type}posYCat",_cat.gameObject.transform.position.y);
        PlayerPrefs.SetFloat($"{type}rotationZCat",_cat.gameObject.transform.rotation.z);
    }
    private void SaveProgressBrick(int indexBrick, float posX,float posY,float rotationZ)
    {
        var type = _onBot ? 2:1;
        PlayerPrefs.SetFloat($"{type}posX{indexBrick}",posX);
        PlayerPrefs.SetFloat($"{type}posY{indexBrick}",posY);
        PlayerPrefs.SetFloat($"{type}rotationZ{indexBrick}",rotationZ);
    }

    enum hod
    {
        player1,
        player2,
        bot
    }
}