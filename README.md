# Micro-PubSub

## Introdução

Iniciando os estudos sobre [System.Thread.Channels](https://docs.microsoft.com/en-us/dotnet/api/system.threading.channels), decidi criar um "micro" pub/sub para implementar uma solução utilizando os recursos da biblioteca. A princípio será apenas uma biblioteca para criar _produtores_ que podem publicar _"mensagens"_ em tópicos, e _consumidores_ que irão receber como _"eventos"_ essas mensagens.

Defini alguns objetivos:
- Gerência dos tópico;
- Encapsular publicação e consumo das mensagens (talvez não usando diretamente a classe _Channels<T>_);
- usar System.Thread.Channels para I/O das mensagens;

## Alguns pensamentos

Quando vi o anuncio da biblioteca de _Channels_ e após ler alguns artigos, imaginei que esse cenário seria um bom caso de uso. Pensando nos conceitos de pub/sub, de forma simplista, um agente que publica em uma fila onde 