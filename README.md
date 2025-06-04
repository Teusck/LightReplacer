# LightReplacer

Projeto atualizado para C# e .NET. Agora o utilitário pode ser compilado e executado em qualquer plataforma suportada pelo .NET 8.

## Requisitos
- .NET 8 SDK

## Como compilar
```bash
cd src/LightReplacerCS
 dotnet build
```

## Como usar
```
dotnet run -- <arquivos> [-n nome] [--upper|--lower] [--translate origem destino]
```

Exemplo:
```
dotnet run -- arquivo.txt -n NovoNome --upper --translate pt en
```
