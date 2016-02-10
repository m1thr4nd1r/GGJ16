using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    
    public Text feedback, stage, inventory;

    enum Locations { Sala, Porao, Cozinha, Quarto, Hall, QuartoP, Banheiro, Sotao };
    enum Items { Potion, Cake, Picture, Mirror, Flower, Item, Arrived, Action, PotionS, CakeS, PictureS, MirrorS, FlowerS, ItemS };
    enum Actions { Left, Right, Up, Down };
    string[,] feed;
    string last;
    int[,] map;
    int current, selected, joyqnt, joynum;
    bool[] used;
    bool flag;
    Dictionary<int, string> scene;
    Dictionary<string, string> translate;
    List<string> inv;

    void Start () {
        
        joyqnt = Input.GetJoystickNames().Length;
        joynum = -1;
        nextPlayer(joynum);
        flag = true;
        current = -2;
        scene = new Dictionary<int, string>();
        translate = new Dictionary<string, string>();
        selected = -1;
        last = "";
        inv = new List<string>(3);
        feedback.text = "";
        used = new bool[6];
        for (int i = 0; i < 6; i++)
            used[i] = false;
        feedback.text = "Instruções de jogo: Para jogar com mais de 1 controle.\n Cada jogador tem um turno para executar uma ação. Use o direcional analógico para se mover entre os cômodos. Pressione X para interagir com o cômodo ou usar ítens equipados. Pressione A,B ou Y para equipar itens do inventário.";
        stage.text = "Start Game ?";
        inventoryAdd("Pressione A para começar");
        inventoryAdd("Pressione B para sair");
        inventoryAdd("Hmm, talvez mais tarde...");
        inventory.text = inventoryPrint();
        fillData();
    }

    void inventoryAdd(string name)
    {
        inv.Add(name);
    }

    string inventoryPrint()
    {
        string res = "";
        for (int i = 0; i < inv.Count; i++)
            res += (i + 1).ToString() + " - " + inv[i] + "\n";
        return res;  
    }

    public int nextPlayer(int last)
    {
        Debug.Log("Number of Joys: " + joyqnt);
        Debug.Log("Chosen Joystick: " + joynum);
        do
            joynum = UnityEngine.Random.Range(1,joyqnt+1);
        while (joynum == last);

        return joynum;
    }

    void action(string choice)
    {
        string res = "";
        int temp = -1;
        
        switch (choice)
        {
            case "Left":
                temp = map[current, (int)Actions.Left];
                if (temp >= 0)
                {
                    current = temp;
                    scene.TryGetValue(current, out res);
                    stage.text = res;
                    feedback.text = feed[current, (int)Items.Arrived];
                }
                break;
            case "Right":
                temp = map[current, (int)Actions.Right];
                if (temp >= 0)
                {
                    current = temp;
                    scene.TryGetValue(current, out res);
                    feedback.text = feed[current, (int)Items.Arrived];
                    stage.text = res;
                }
                break;
            case "Up":
                temp = map[current, (int)Actions.Up];
                if (temp >= 0)
                {
                    current = temp;
                    scene.TryGetValue(current, out res);
                    feedback.text = feed[current, (int)Items.Arrived];
                    stage.text = res;
                }
                break;
            case "Down":
                temp = map[current, (int)Actions.Down];
                if (temp >= 0)
                {
                    current = temp;
                    scene.TryGetValue(current, out res);
                    feedback.text = feed[current, (int)Items.Arrived];
                    stage.text = res;
                }
                break;
            case "Use":
                if (current > -1)
                {
                    if (selected > -1)
                    {
                        if (selected == (int)Items.PotionS && current == (int)Locations.Sala)
                        {
                            
                            inv.Remove("Poção");
                            inventory.text = inventoryPrint();
                            map[current, (int)Actions.Up] = (int)Locations.Hall;
                            scene.Remove((int)Locations.Sala);
                            scene.Add((int)Locations.Sala, "Uma sala como outra qualquer. As portas a Sul, Norte, Leste e Oeste estão liberadas...");
                            scene.TryGetValue((int)Locations.Sala, out res);
                            stage.text = res;
                            res = feed[current, selected];
                            feed[current, (int)Items.Action] = "Uma Sala como outra qualquer";
                            used[(int)Items.Potion] = true;
                            selected = -1;
                        }
                        else if (selected == (int)Items.FlowerS && current == (int)Locations.QuartoP)
                        {
                            
                            inv.Remove("Flor");
                            inv.Add("Joia");
                            inventory.text = inventoryPrint();
                            res = feed[current, selected];
                            used[(int)Items.Flower] = true;
                            selected = -1;
                        }
                        else if (current == (int)Locations.Porao)
                        {
                            if (selected == (int)Items.ItemS)
                            {
                                
                                inv.Remove("Joia");
                                inventory.text = inventoryPrint();
                                res = feed[current, selected];
                                used[(int)Items.Item] = true;
                                selected = -1;
                            }
                            else if (selected == (int)Items.MirrorS)
                            {
                                
                                inv.Remove("Espelho");
                                inventory.text = inventoryPrint();
                                res = feed[current, selected];
                                used[(int)Items.Mirror] = true;
                                selected = -1;
                            }
                            else if (selected == (int)Items.PictureS)
                            {
                                
                                inv.Remove("Foto");
                                inventory.text = inventoryPrint();
                                res = feed[current, selected];
                                used[(int)Items.Picture] = true;
                                selected = -1;
                            }

                            if (used[(int)Items.Item] &&
                                used[(int)Items.Picture] &&
                                used[(int)Items.Mirror])
                            {
                                //res = "Cabo o Gami";
                                Invoke("EndGame", 2);
                            }
                        }
                        else
                        {
                            res = feed[current, selected];
                            
                        }
                        }
                    else
                    {
                        res = feed[current, (int)Items.Action];
                        if (current == (int)Locations.Cozinha && !inv.Contains("Poção") && !used[(int)Items.Potion])
                        {
        
                            inv.Add("Poção");
                            feed[current, (int)Items.Action] = "Uma sala como outra qualquer";
                        }
                        else if (current == (int)Locations.Quarto && !inv.Contains("Foto") && !used[(int)Items.Picture])
                        {
        
                            inv.Add("Foto");
                            //feed[current, ()]
                        }
                        else if (current == (int)Locations.Hall && !inv.Contains("Espelho") && !used[(int)Items.Mirror])
                        {
        
                            inv.Add("Espelho");
                            //feed[current, ()]
                        }
                        else if (current == (int)Locations.Sotao && !inv.Contains("Flor") && !used[(int)Items.Flower])
                        {
        
                            if (flag)
                            {
                                feed[current, (int)Items.Action] = "Você encontra, entre páginas amareladas do grande livro da família, um galho com três botões de rosa já desidratados. Você guarda as flores consigo.";
                                flag = false;
                            }
                            else
                                inv.Add("Flor");
                        }

                        inventory.text = inventoryPrint();
                    }

                    feedback.text = res;
                }
                break;
            case "1":
                if (current > -1)
                {
                    
                    translate.TryGetValue(inv[0], out res);
                    //Debug.Log(res + "|" + selected);
                    selected = (int)Enum.Parse(typeof(Items), res);
                    feedback.text = feed[current, selected];

                    if (selected >= 0)
                        res += "S";
                    selected = (int)Enum.Parse(typeof(Items), res);
                }
                else if (current == -2)
                {
                    
                    inventory.text = "";
                    inv.Clear();
                    feedback.text = "Seu pai, um poderoso feiticeiro, irritado com as brigas constantes condena você e seus irmãos a conviverem no mesmo corpo por uma semana a menos que vocês consigam executar colaborativamente um ritual para apaziguar seus antepassados incomodados com as intrigas. Você está no porão da sua casa.";
                    scene.TryGetValue((int)Locations.Porao, out res);
                    stage.text = res;
                    current = (int)Locations.Porao;
                }
                break;
            case "2":
                if (current > -1)
                {
                    
                    translate.TryGetValue(inv[1], out res);
                    //Debug.Log(res + "|" + selected);
                    selected = (int)Enum.Parse(typeof(Items), res);
                    feedback.text = feed[current, selected];

                    if (selected >= 0)
                        res += "S";
                    selected = (int)Enum.Parse(typeof(Items), res);
                }
                else if (current == -2)
                    Application.Quit();
                break;
            case "3":
                if (current > -1)
                {
                    
                    translate.TryGetValue(inv[2], out res);
                    //Debug.Log(res + "|" + selected);
                    selected = (int)Enum.Parse(typeof(Items), res);
                    feedback.text = feed[current, selected];

                    if (selected >= 0)
                        res += "S";
                    selected = (int)Enum.Parse(typeof(Items), res);
                }
                break;
        }
        nextPlayer(joynum);
        //Debug.Log(res+ " | " + current + " | " + temp);
    }

    void Update()
    {
       
        //Debug.Log("Horizontal: " + Input.GetAxis("Horizontal" + joynum));
        //Debug.Log("Vertical: " + Input.GetAxis("Vertical" + joynum));

        if (Input.GetAxis("Horizontal" + joynum) == 0 ||
            Input.GetAxis("Vertical" + joynum) == 0)
            last = "";

        if (Input.GetAxis("Horizontal" + joynum) < 0)
            action("Left");
        else if (Input.GetAxis("Horizontal" + joynum) > 0)
            action("Right");
        else if (Input.GetAxis("Vertical" + joynum) < 0)
            action("Up");
        else if (Input.GetAxis("Vertical" + joynum) > 0)
            action("Down");
        else if (Input.GetKeyDown("joystick " + joynum + " button 0"))
            action("1");
        else if (Input.GetKeyDown("joystick " + joynum + " button 1"))
            action("2");
        else if (Input.GetKeyDown("joystick " + joynum + " button 2"))
            action("Use");
        else if (Input.GetKeyDown("joystick " + joynum + " button 3"))
            action("3");
        else if (Input.GetButtonDown("Submit"))
            SceneManager.LoadScene(0);
    }

    void EndGame()
    {
        feedback.text = "O ritual está completo. Os seus antepassados foram apaziguados e vocês são três novamente.";
        stage.text = "Credits: Victor Cayres, Victor Santos, Vitor Rabelo";
        inventory.text = "Press \'Start\' to restart";
    }

    void fillData()
    {
        map = new int[8, 4];
        feed = new string[8, 14];

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 4; j++)
                map[i, j] = -1;
            for (int j = 0; j < 14; j++)
                feed[i, j] = "";
        }  
        map[(int)Locations.Porao, (int)Actions.Up] = (int)Locations.Sala;
        map[(int)Locations.Sala, (int)Actions.Right] = (int)Locations.Quarto;
        map[(int)Locations.Sala, (int)Actions.Left] = (int)Locations.Cozinha;
        map[(int)Locations.Sala, (int)Actions.Down] = (int)Locations.Porao;
        map[(int)Locations.Cozinha, (int)Actions.Right] = (int)Locations.Sala;
        map[(int)Locations.Quarto, (int)Actions.Left] = (int)Locations.Sala;
        map[(int)Locations.Hall, (int)Actions.Up] = (int)Locations.Sotao;
        map[(int)Locations.Hall, (int)Actions.Down] = (int)Locations.Sala;
       // map[(int)Locations.Hall, (int)Actions.Right] = (int)Locations.Banheiro;
        map[(int)Locations.Hall, (int)Actions.Left] = (int)Locations.QuartoP;
        map[(int)Locations.QuartoP, (int)Actions.Right] = (int)Locations.Hall;
        map[(int)Locations.Banheiro, (int)Actions.Left] = (int)Locations.Hall;
        map[(int)Locations.Sotao, (int)Actions.Down] = (int)Locations.Hall;

        feed[(int)Locations.Porao, (int)Items.Potion] = "Vocês estão com a poção nas mãos. No frasco está escrito: \"Beba-me para encolher.\"";
        feed[(int)Locations.Porao, (int)Items.Picture] = "Você está com as fotos nas mãos.";
        feed[(int)Locations.Porao, (int)Items.Mirror] = "Você está com o espelho nas mãos.";
        feed[(int)Locations.Porao, (int)Items.Flower] = "Você está com a flor seca nas mãos.";
        feed[(int)Locations.Porao, (int)Items.Item] = "Você está com a joia nas mãos.";
        feed[(int)Locations.Porao, (int)Items.Action] = "Você se aproxima da concha-caldeirão e nota que seu fundo é triangular.";
        feed[(int)Locations.Porao, (int)Items.PotionS] = "Melhor não disperdiçar isto.";
        feed[(int)Locations.Porao, (int)Items.PictureS] = "Você colocou a foto em uma das pontas do triângulo ao fundo da concha caldeirão";
        feed[(int)Locations.Porao, (int)Items.MirrorS] = "Você colocou o espelho em uma das três pontas do triâgulo ao fundo da concha-caldeirão.";
        feed[(int)Locations.Porao, (int)Items.FlowerS] = "Não parece uma boa ideia.";
        feed[(int)Locations.Porao, (int)Items.ItemS] = "Você colocou o ítem mágico em uma das pontas do triângulo ao fundo da concha caldeirão";
        feed[(int)Locations.Porao, (int)Items.Arrived] = "Você entrou no porão";

        feed[(int)Locations.Sala, (int)Items.Action] = "Não parece muito inteligente acordar um gato deste tamanho.";
        feed[(int)Locations.Sala, (int)Items.Potion] = "Você está com a poção nas mãos.";
        feed[(int)Locations.Sala, (int)Items.PotionS] = "Você deu a poção de encolher ao gato. Pequeno asim ele parece bem mais amigável.";
        feed[(int)Locations.Sala, (int)Items.Arrived] = "Você entrou na Sala";
        feed[(int)Locations.Sala, (int)Items.Picture] = "Você está com as fotos nas mãos.";
        feed[(int)Locations.Sala, (int)Items.Mirror] = "Você está com o espelho nas mãos.";
        feed[(int)Locations.Sala, (int)Items.Flower] = "Você está com a flor seca nas mãos.";
        feed[(int)Locations.Sala, (int)Items.Item] = "Você está com a joia nas mãos.";

        feed[(int)Locations.Cozinha, (int)Items.Action] = "Você pega o poção sobre a mesa e vê que nela está escrito:\"Beba-me para encolher.\"";
        feed[(int)Locations.Cozinha, (int)Items.Potion] = "Você está com a poção nas mãos. No frasco está escrito: \"Beba-me para encolher.\"";
        feed[(int)Locations.Cozinha, (int)Items.PotionS] = "Encolher quando há um gato do tamanho de um tigre na sala não parece a melhor opção";
        feed[(int)Locations.Cozinha, (int)Items.Picture] = "Você está com as fotos nas mãos.";
        feed[(int)Locations.Cozinha, (int)Items.Mirror] = "Você está com o espelho nas mãos.";
        feed[(int)Locations.Cozinha, (int)Items.Flower] = "Você está com a flor seca nas mãos.";
        feed[(int)Locations.Cozinha, (int)Items.Item] = "Você está com a joia nas mãos.";
        feed[(int)Locations.Cozinha, (int)Items.Arrived] = "Você entrou na cozinha";

        feed[(int)Locations.Quarto, (int)Items.Action] = "Você pegou a foto e guardou consigo.";
        feed[(int)Locations.Quarto, (int)Items.Potion] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Quarto, (int)Items.Picture] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Quarto, (int)Items.Mirror] = "Você pegou a foto e guardou consigo.";
        feed[(int)Locations.Quarto, (int)Items.Flower] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Quarto, (int)Items.Item] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Quarto, (int)Items.Arrived] = "Você entrou no seu quarto";

        feed[(int)Locations.Hall, (int)Items.Action] = "Você olha o seu reflexo e não consegue se reconhecer, decide guardar consigo umas gotas de espelho no intuito de tentar se acostumar com a nova face.";
        feed[(int)Locations.Hall, (int)Items.Arrived] = "Você entrou no hall superior";
        feed[(int)Locations.Hall, (int)Items.Potion] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Hall, (int)Items.Picture] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Hall, (int)Items.Mirror] = "Você pegou a foto e guardou consigo.";
        feed[(int)Locations.Hall, (int)Items.Flower] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Hall, (int)Items.Item] = "O que você pretende fazer com isso?";

        feed[(int)Locations.Sotao, (int)Items.Action] = "No alto da página aberta ha uma ilustração de um peixe com três caudas e uma única cabeça. Abaixo os escritos: \"Num megulho chegará aos céus e será livre quando olhando a seus irmãos enxergar a si mesmo.\"";
        feed[(int)Locations.Sotao, (int)Items.Arrived] = "Você entrou no Sótão";
        feed[(int)Locations.Sotao, (int)Items.Potion] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Sotao, (int)Items.Picture] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Sotao, (int)Items.Mirror] = "Você pegou a foto e guardou consigo.";
        feed[(int)Locations.Sotao, (int)Items.Flower] = "O que você pretende fazer com isso?";
        feed[(int)Locations.Sotao, (int)Items.Item] = "O que você pretende fazer com isso?";

        feed[(int)Locations.QuartoP, (int)Items.Action] = "Sua mãe diz que não se mete nos castigos de seu pai, assim como ele não se mete nos dela";
        feed[(int)Locations.QuartoP, (int)Items.FlowerS] = "Sua mãe diz: \"Seu pai me deu essas rosas quando disse a ele que teríamos trigêmios.Tomem aqui esta jóia, ela está na nossa família há milênios. Olhem bem para esses peixes que sendo três conseguem ser um e tentem aprender algo com isso. \" Você guarda consigo a jóia da família.";
        feed[(int)Locations.QuartoP, (int)Items.Arrived] = "Você entrou no quarto de seus pais";
        feed[(int)Locations.QuartoP, (int)Items.Potion] = "O que você pretende fazer com isso?";
        feed[(int)Locations.QuartoP, (int)Items.Picture] = "O que você pretende fazer com isso?";
        feed[(int)Locations.QuartoP, (int)Items.Mirror] = "Você pegou a foto e guardou consigo.";
        feed[(int)Locations.QuartoP, (int)Items.Flower] = "O que você pretende fazer com isso?";
        feed[(int)Locations.QuartoP, (int)Items.Item] = "O que você pretende fazer com isso?";

        scene.Add((int)Locations.Porao, "As paredes do porão são madreperoladas, há no centro do cômodo uma grande concha-caldeirão, a única saída está a norte. Sobre escadas a Norte, uma porta dá para a sala da casa.");
        scene.Add((int)Locations.Sala, "Uma sala como outra qualquer, exceto pelo gato gigante dormindo à beira da escada a Norte. As portas a Sul, Leste e Oeste estão liberadas. Ele costumava ser tão pequeno que quase cabia na sua mão. Parece que seu pai não quer facilitar as coisas para você e seus irmãos...");
        scene.Add((int)Locations.Cozinha, "Há apenas uma porta a Leste. Na cozinha, o de sempre.Geladeira, fogão, condimentos e poções.Opa!Uma delas chamou sua atenção, não está no armário, mas na mesa.E tem algo escrito no frasco...");
        scene.Add((int)Locations.Quarto, "Nada diferente do usual. Como vocês nunca conseguiram se entender sobre o posicionamento das camas. Os colchões giram lenta e constantemente no ar. Vocês olham saudosamente para uma foto dos tempo em que vocês eram três. Há porta de onde você veio está a oeste.");
        scene.Add((int)Locations.Hall, "Há um grande espelho mágico feito de um líquido viscoso e prateado. Portas a Norte, a Oeste e a Sul.");
        scene.Add((int)Locations.Sotao, "O Sótão guarda uma extensa biblioteca. Um livro entretanto ganha destaque, está aberto sobre um púpito, o sol que entra pela janela ilumina as páginas bordadas em dourado. Há uma porta ao sul.");
        scene.Add((int)Locations.QuartoP, "Sua mãe está sentada à escrivanhinha trabalhando em fórmulas alquímicas muito complexas. Há apenas uma porta a Leste");

        translate.Add("Poção", "Potion");
        translate.Add("Foto", "Picture");
        translate.Add("Espelho", "Mirror");
        translate.Add("Flor", "Flower");
        translate.Add("Joia", "Item");
    }
}
