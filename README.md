# Projekt Systemu Bankowego

# Pliki

System Bankowy przechowuje Twoje dane w bazie danych sql

## Logowanie

Dane logowania są bezpiecznie przechowywane


Diagram algorytmu:

```mermaid
graph LR
A(START) --> B[Logowanie]
B --> C{Logowanie}
C -- udało się --> D{Wybór funkcji}
C -- nie udało się --> B
E((Dane konta)) --> D
D --> F_1[Stan konta]
D --> F_2[Wypłata]
D --> X[Wyjdź z programu]
F_1 -- udało się --> D{Wybór funkcji}
F_2 -- udało się --> D{Wybór funkcji}
X --> Z(STOP)
```
