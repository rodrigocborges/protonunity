# Proton

![Exemplo simples com 2 jogadores](https://media.githubusercontent.com/media/rodrigocborges/protonunity/master/Gifs/teste_2_jogadores_sem_delay.gif)
*Exemplo simples com 2 jogadores - sincronização de posição, escala, rotação e texto da nametag*

A maioria das arquiteturas de rede que são utilizadas em jogos multiplayer online são cliente-servidor. O presente projeto 
trata de construir uma alternativa ligada a implementação da arquitetura peer-to-peer para:

- Estudo da viabilidade de implementação;
- Caráter comparativo entre arquiteturas;

## Objetivo geral
Propor um framework open source para multiplayer online de jogos na Unity usando WebRTC.

## Informações extras
- Está sendo usado a biblioteca Javascript PeerJS para maior abstração das classes e funções da API da WebRTC
- Está sendo feito a comunicação de Javascript com C# através de JSLib (já nativo da Unity)
- Versão da Unity sendo usada: 2021.3.13f1
- Já existe um pacote experimental da Unity para comunicação com WebRTC, mas nos testes não foi possível comunicar mais de 2 jogadores. Além disso, a complexidade de implementação era alta
- Está sendo feito um servidor de sinalização simples que para cada peer novo conectado é enviado esse seu ID para o servidor, que salva essa informação num arquivo JSON para enviar para todos os peers conectados para realizar conexões entre si. Nenhum dado do jogo é enviado para ele, apenas sinalização (encontrar os outros peers na rede).
- Plataforma para fazer o build: apenas WebGL

## Documentação
Para acessar a documentação, acesse o link: <https://protonunity.readthedocs.io>

## Observação
Este projeto contém também um exemplo de jogo usando o Photon Unity Networking para caráter de testes entre frameworks e para construções de funções análogas.

## Créditos
Desenvolvido por Rodrigo Borges - curso de Engenharia de Computação na Universidade Federal do Rio Grande (FURG)
