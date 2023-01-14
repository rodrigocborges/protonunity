
# Documentação Proton

Framework open source (<https://github.com/rodrigocborges/protonunity>) para desenvolvimento de jogos exportados 
para WebGL online usando a tecnologia WebRTC com a Unity. Portanto os jogos todos têm seu build focados somente 
para web.

## O que é WebRTC?
WebRTC é uma tecnologia que permite a comunicação em tempo real entre diferentes dispositivos na internet. Ele foi desenvolvido para permitir a criação de aplicações de vídeo e voz em tempo real sem a necessidade de plugins ou aplicativos adicionais. WebRTC usa protocolos padrão da internet para transmitir dados de áudio e vídeo de maneira segura e eficiente, o que o torna uma opção popular para aplicativos de mensagens instantâneas, videoconferências e chamadas de voz. Ele também pode ser usado para compartilhar arquivos e dados em tempo real entre dispositivos, como a transferência de arquivos entre usuários.

Site oficial: <https://webrtc.org/>

## Instalação
O processo de instalação é simples, basta fazer um clone do repositório no GitHub, exemplo:

`git clone https://github.com/rodrigocborges/protonunity.git`

Com isso você já terá todo projeto Unity com o sistema integrado. A partir daí é só construir seu jogo 😀!

*Caso você não tenha um cliente git no seu computador, basta acessar o link do GitHub e baixar como .zip, aí basta você extrair para uma pasta e fazer o mesmo processo, abrir na Unity e estará com tudo instalado!*

## Funcionamento de forma geral
Há um template customizado do build do WebGL no projeto da Unity que ao exportar para web, no arquivo index.html, há uma referência externa ao script
do PeerJS, uma biblioteca que abstrai e simplifica muito a utilização da tecnologia WebRTC. Lá na Unity usando um arquivo .jslib é feito
uma ponte das funções Javascript com C# linguagem nativa e principal para programação dos jogos na engine. Portanto, todo processo
do WebRTC: conexão entre os peers e também a criação e o envio de mensagens, é feito do via Javascript.

# Utilização

Para entender os conceitos de networking para construir um jogo multiplayer online na Unity usando esse framework, devemos aplicar alguns passos. Se você quiser adicionar um jogador onde só você controla (comportamento esperado) você precisa adicionar nesse objeto o seguinte script ao GameObject: `EntityIdentity`.

Esse script é responsável por conter a identidade de tal entidade na rede, ou seja, contém seu GUID (identificador único global) e também se é de autoridade sua ou não - se o GameObject é seu ou não. Isso vai ser importante para se você quiser aplicar uma movimentação nesse objeto, você movimentar só você mesmo e não você e os outros players, isso individualiza o objeto em cena.

Agora, se você quiser sincronizar informações de posição, escala e rotação (ou todas elas) basta adicionar um script chamado `SyncTransform`. Isso é usado para você movimentar seu jogador localmente e poder enviar essa informação via rede sincronizando isso e também acontece o inverso, você poder enxergar a movimentação do outro jogador de forma sincronizada na sua gameplay. 

Para usar ambos os scripts citados acima é necessário criar um terceiro script para justamente linkar os dois e fazer o comportamento que esperamos: movimentar o objeto. Portanto, vamos criar um script simples chamado `PlayerNetwork` de exemplo para uma movimentação também simples:

```csharp

using UnityEngine;
using Proton;

public class PlayerNetwork : MonoBehaviour
{
    [SerializeField] private float speed; //Velocidade de movimentaçã do jogador

    private EntityIdentity _identity;
    
    void Awake(){
        _identity = GetComponent<EntityIdentity>();
    }

    void Start(){
        /*
            Aqui é identificado se o jogador é local ou não, se for local (se for de autoridade minha) 
            fica de cor vermelha, caso contrário (outros jogadores), cor azul.
        */
        GetComponent<MeshRenderer>().material.color = _identity.IsMine() ? Color.red : Color.blue;
    }


    void Update()
    {
        //Se for realmente eu, segue o fluxo normal de script para movimentar apenas eu
        if(!_identity.IsMine() || string.IsNullOrEmpty(_identity.GetPeerID()))
            return;

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        //Aplica uma movimentação de andar para os lados e para frente e para trás
        transform.Translate(new Vector3(horizontal * Time.deltaTime * speed, 0, vertical * Time.deltaTime * speed));
    }
}

```

Após a criação do script, basta adicionar esse ao objeto do Player e criar um prefab desse GameObject para podermos instanciá-lo ao entrar/criar uma partida multiplayer. 

Para começar os testes, vamos criar um GameObject em branco e renomeá-lo (isso é opcional, apenas mais organização) para ProtonManager, clicar nele e adicionar o script de mesmo nome `ProtonManager`. Nele vai ser feito todo o gerenciamento de rede, a comunicação nos canais de dados, envio e recebimento de informações, entrada dos jogadores e também o instanciamento dos jogadores. Portanto, basta preencher a variável `playerPrefab` com o prefab que foi criado anteriormente do Player. 