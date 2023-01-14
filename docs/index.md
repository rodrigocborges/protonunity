
# Documentação Proton

Framework open source (<https://github.com/rodrigocborges/protonunity>) para desenvolvimento de jogos exportados 
para WebGL online usando a tecnologia WebRTC com a Unity. Portanto os jogos todos têm seu build focados somente 
para web.

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