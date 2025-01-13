# Weiterentwicklung (Fallbeschreibung)

Die LBank hat nun ein grobes Back- und Frontend. Es fehlen jedoch noch einige Features, um die Anwendung zu vervollständigen. 

Im ersten Schritt bauen wir noch die "Buchen" Funktion ein. Diese soll es ermöglichen, dass ein Benutzer eine Buchung tätigen kann.

## Teilaufgabe 1: Analyse

Die «Buchen»-Funktion soll einen gegebenen Betrag von einem Konto auf ein anderes übertragen. Zusätzlich soll es nicht möglich sein, mit einer Buchung den Kontostand unter 0 zu bringen. Sie haben folgende Operationen zur
Verfügung:
- Betrag von Konto X lesen
- Betrag von Konto X ändern
- Konto X in der Datenbank ändern

Schreiben Sie den Ablauf im Backend auf, wenn Sie einen Betrag B von Konto «Quelle» auf Konto «Ziel» übertragen möchten

```
Transaktion starten: Eine Transaktion wird begonnen.
Konten laden: Quelle und Ziel werden aus der Datenbank geholt.
Betrag ändern: Betrag wird von Quelle abgezogen und zu Ziel hinzugefügt.
Änderungen speichern: Die neuen Kontostände werden gespeichert.
Transaktion abschließen: Transaktion wird abgeschlossen (Commit).
Fehler behandeln: Bei Fehlern wird die Transaktion zurückgesetzt (Rollback).
```

Wo kann es bei diesem Ablauf bei Multiuserapplikationen zu Problemen kommen? Finden Sie mindestens 3 Probleme.

Was ist die Lösung dieses Problems?

```
Problem 1: Race Condition
Zwei Benutzer greifen gleichzeitig auf das gleiche Konto zu.

Lösung: Nutze Transaktionen, damit alles atomar abläuft.

Problem 2: Dirty Read
Ein Benutzer liest unbestätigte Daten.

Lösung: Setze das Isolationslevel auf Read Committed.

Problem 3: Lost Update
Änderungen eines Benutzers überschreiben andere.

Lösung: Verwende Locking, um parallele Zugriffe zu verhindern.
```

## Teilaufgabe 2: Implementation

Implementieren Sie im Backend den Controller und das Model.

Wenn eine Transaktion misslingt, dann sollten Sie sie diese neu starten. Dafür müssen Sie die ganze Operation in eine Schleife einbauen. Wie das gemacht wird, sehen Sie in LedgersModel.SelectOne().

Testen Sie das Backend mit der JSON-MAN Website, indem Sie zuerst einloggen und dann den Bookings-API-Endpunkt aufrufen.

Implementieren Sie im Frontend den API-Service und testen Sie die Buchung in der Applikation.

## Weitere Features

Nun sind sie gefragt, bauen Sie weitere Features ein, die Sie für sinnvoll halten.
z.b.:
- Konto löschen
- Konto erstellen
- Konto umbenennen
- Kontoübersicht
- ...

Diese weiteren Features werden als Teil der LB bewertet.