# Weiterentwicklung

Sie haben erkannt, dass es für Multiuser Applikationen meistens Transaktionen und ein Backend braucht. Diese Applikation implementiert einen grossen Teil.

## Aufgabenstellung

Mit dem Backend haben wir uns ja bereits mehrfach auseinander gesetzt. 
Schauen Sie sich jetzt auch mal das ".Web"-Projekt an.

Schauen Sie sich das Projekt an und versuchen Sie folgende Fragen zu beantworten:

Wenn Sie die Verzeichnisnamen anschauen, welchem Architekturmuster folgt
diese Applikation?

Was befindet sich im Verzeichnis «Controllers» und was ist die Aufgabe? ```Verarbeitung von HTTPS anfragen```

Was befindet sich im Verzeichnis «Models» und was ist die Aufgabe? ```Typen werden dort definiert```

Wo wird die Datenbank gemacht und die Daten geseeded? ```Migrations```

Was für Benutzer mit was für Passworten und «Rollen» werden erstellt?

Schauen Sie mit SQL Management Studio die erstellten Tabellen und Daten an.

Was wird als Passwort gespeichert und warum?

Welche URL hat der Login-API-Controller-Endpunkt und was für eine Methode
verwendet er? ```api/v1/LoginController, Post```

Welche URL hat der LBankInfo-API-Controller-Endpunkt und was für eine
Methode verwendet er? ```api/v1/BankInfoController, Get```

Welche URL hat die Liste aller ledgers im API-Controller-Endpunkt und was für eine Methode verwendet sie? ```api/v1/LedgersController, Get```

Wo wird der Benutzername und das Passwort überprüft? ```LoginController```
$

### Teilaufgabe 1: Authentifizierung

Wir spielen nun Frontend. Sie können gerne auch den richtigen POSTMAN verwenden.

Loggen Sie sich ein, indem Sie ein JWT anfordern

Kopieren Sie sich das JWT.

Fordern Sie nun eine Liste aller Ledgers an.

Machen Sie den gleichen Aufruf ohne das Token im Header, was passiert?
```
Ohne Token kommt 401 Unauthorized
Mit token kommt Erbegnis
```
Rufen Sie auch den Infopunkt mit und ohne Token auf, was ist der Unterschied?
```
Ohne Token nur Name und Version
Mit Token noch mehr Infos
```

A: Mit Token wird die Benutzerinfo zurückgegeben.

### Teilaufgabe 2: Generelles Verständnis des Backends

Verfolgen Sie die Aufrufe von /api/v1/login, /api/v1/lbankinfo und /api/v1/ledgers durch die Applikation. Sie können auch die Debuggingfunktion dafür
verwenden.

Schreiben Sie Fragen und Unklarheiten auf und versuchen Sie diese mit Ihren
Mitlernenden oder der Lehrperson zu klären.
