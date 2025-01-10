# Transaktionen

Die Relevanz von Transaktionen wurde gezeigt. Sie haben einen lauffähigen MsSQL-Server auf Ihrem Rechner und die Beispieldatenbank ist vorhanden.
Wenn Sie die Beispieldatenbank noch nicht erstellt haben, können Sie
folgendermassen vorgehen:

Kopieren Sie folgendes SQL-Statement in das SQL Server Management Studio und führen Sie es aus:

```sql
USE [master]
GO

CREATE DATABASE [l_bank]
GO

USE [l_bank]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ledgers](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](50) NOT NULL,
	[balance] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET IDENTITY_INSERT [dbo].[ledgers] ON 
GO
INSERT [dbo].[ledgers] ([id], [name], [balance]) VALUES (16, N'Manitu AG', 100.0000)
GO
INSERT [dbo].[ledgers] ([id], [name], [balance]) VALUES (17, N'Chrysalkis GmbH', 200.0000)
GO
INSERT [dbo].[ledgers] ([id], [name], [balance]) VALUES (18, N'Smith & Co KG', 300.0000)
GO
SET IDENTITY_INSERT [dbo].[ledgers] OFF
GO

```

## Aufgabenstellung
In dieser Aufgabe werden Transaktionen theoretisch und praktisch erkundet.
### Teilaufgabe 1 Theoretische Analyse
In folgender Tabelle läuft die Zeit von oben nach unten. P0 bis P2 sind unterschiedliche Prozesse (Programme, User, …).
a, b, c sind unterschiedliche Datensätze.

![](2024-11-22-09-52-36.png)


Gehen Sie davon aus, dass wenn ein Prozess einen Datensatz (a, b oder c) liest
und danach schreibt, er damit arbeitet und die Datenbank zu diesem Zeitpunkt
inkonsistent ist.
Versuchen Sie zu identifizieren, wo folgende Probleme auftreten können:
- Dirty Read
- Non-repeatable read
- Phantom Read
- Sich gegenseitig überschreibende Daten

Welchen Isolationslevel brauchen Sie, um diese Probleme zu verhindern?

Dirty Reads treten auf, wenn z. B. P1 bei Zeit 2 b schreibt und P0 oder P2 diesen Wert liest, bevor die Transaktion abgeschlossen ist. Non-repeatable Reads passieren, wenn P0 a liest (Zeit 0) und P2 später a überschreibt (Zeit 8). Um alle Probleme zu vermeiden, sollte das Isolationslevel Serializable verwendet werden.

### Teilaufgabe 2: Theoretische Analyse L-Bank

Nehmen Sie folgenden Ablauf für die L-Bank:

![](2024-11-22-09-57-22.png)

Versuchen Sie zu identifizieren, wo folgende Probleme auftreten können:
- Dirty Read
- Non-repeatable read

Da keine Datensätze erstellt oder gelöscht werden, gibt es keine Phantom
Reads Welchen Isolationslevel brauchen Sie, um diese Probleme zu verhindern?

P0 und P1: REPEATABLE READ, um Non-repeatable Reads und Dirty Reads zu vermeiden.

P2: Da P2 die Summierung aller Werte in der Datenbank ausführt, benötigt es mindestens REPEATABLE READ, um konsistente Daten zu garantieren und Änderungen während der Summierung zu verhindern.

### Teilaufgabe 3: Praktische Analyse L-Bank

Hier simulieren wir diesen Ablauf mit und ohne Transaktionen «von Hand».
Öffnen Sie das «Microsoft SQL Server Management Studio».

Bereiten Sie folgende Dinge vor:

- ID des ersten Datensatz in der Tabelle ledgers (a): ```1```
- ID des zweiten Datensatz in der Tabelle ledgers (b): ```2```
- SQL-Befehl um alles Geld zu summieren:
```sql
SELECT SUM(balance) AS total_balance FROM ledgers;
```
- SQL-Befehl um den Kontostand von a auszulesen:
```sql
SELECT balance FROM ledgers WHERE id = 1;
```
- SQL-Befehl um den Kontostand von b auszulesen:
```sql
SELECT balance FROM ledgers WHERE id = 2;
```
- SQL-Befehl um den Kontostand von a neu zu schreiben:
```sql
UPDATE ledgers SET balance = <neuer_wert> WHERE id = 1;
```
- SQL-Befehl um den Kontostand von b neu zu schreiben:
```sql
UPDATE ledgers SET balance = <neuer_wert> WHERE id = 2;
```
- Summe des Geldes zu Beginn:
```sql
-- Nach dem ersten Abfragen der Summen: 
SELECT SUM(balance) FROM ledgers;
```
- Öffnen Sie drei Fenster für P0, P1 und P3:

Schreiben Sie statt den Anweisungen nun die SQL-Befehle in die Tabelle.
- P0 soll 20 von a zu b buchen.
- P1 soll 30 von b zu a buchen.

![](2024-11-22-10-07-21.png)

| Zeit | P0 | P1 | P2 |
| --- | --- | --- | --- |
| 0 | SELECT balance FROM ledgers WHERE id = 1; | | |
| 1 | | SELECT balance FROM ledgers WHERE id = 1; | |
| 2 | SELECT balance FROM ledgers WHERE id = 2; | | |
| 3 | UPDATE ledgers SET balance = <neuer_balance_b> WHERE id = 2; | SELECT balance FROM ledgers WHERE id = 2; | SELECT SUM(balance) AS total_balance FROM ledgers; |
| 4 | UPDATE ledgers SET balance = <neuer_balance_a> WHERE id = 1; | | |
| 5 | | UPDATE ledgers SET balance = <neuer_balance_b> WHERE id = 2; | |
| 6 | | UPDATE ledgers SET balance = <neuer_balance_a> WHERE id = 1; | |

Gehen Sie nun folgendermassen vor:

Löschen Sie vor jedem neuen Befehl immer wieder alle vorherigen Befehle,
sonst werden sie mehrfach ausgeführt.

**Zeit 0:**
Öffnen Sie den zweiten SQL-Tab, fragen Sie den Kontostand von a ab und schreiben Sie ihn auf: ```5374```
```sql
SELECT balance FROM ledgers WHERE id = 1
```

**Zeit 1:**
Öffnen Sie den dritten SQL-Tab, fragen Sie den Kontostand von a ab und schreiben Sie ihn auf: ```5374```

**Zeit 2:**
Öffnen Sie den ersten SQL-Tab, fragen Sie den Kontostand von b ab und schreiben Sie ihn auf: ```-1180```

**Zeit 3:**
P0 soll 20 von a zu b buchen.

Rechnen Sie hier den neuen Kontostand von b aus (den bei Zeit 2 + 20): ```-1180 + 20 = -1160```

Öffnen Sie den ersten SQL-Tab und schreiben Sie dieses Resultat in das Feld balance von b.

Öffnen Sie den zweiten SQL-Tab, fragen Sie den Kontostand von b ab und schreiben Sie ihn auf: ```-1160```

Öffnen Sie das dritte Fenster und fragen Sie die Summe allen Geldes in Ledgers ab: ```11537```

Fällt ihnen etwas auf?

**Zeit 4:**
P0 soll 20 von a zu b buchen.

Rechnen Sie hier den neuen Kontostand von a aus (den bei Zeit 1 - 20): ```5354```

Öffnen Sie den ersten SQL-Tab und schreiben Sie dieses Resultat in das Feldbalance von a.

**Zeit 5:**
P1 soll 30 von b zu a buchen.

Rechnen Sie hier den neuen Kontostand von b aus (den bei Zeit 3 für P1 - 30): ```-1180```

Öffnen Sie den zweiten SQL-Tab und schreiben Sie dieses Resultat in das Feld balance von b.

**Zeit 6:**

Ja, Sie müssen den Befehl nochmals eingeben, sonst weiss die Datenbank nicht, dass dieser Prozess an diesen Daten interessiert ist.

P1 soll 30 von b zu a buchen.

Rechnen Sie hier den neuen Kontostand von a aus (den bei Zeit 0 + 30): ```5384```

Öffnen Sie den zweiten SQL-Tab und schreiben Sie dieses Resultat in das Feld
balance von a.

Und zum Spass: Wie viel Geld haben wir nun in der Datenbank? ```11527```
Fällt ihnen was auf?

### Teilaufgabe 4: Praktische Analyse L-Bank mit Transaktionen

Stellen Sie für die ersten beiden SQL-Tabs die Isolation «Serializeable» ein und für die dritte Tab «Read Committed» (Lösung aus Aufgabe 2).

![](2024-11-22-10-13-27.png)

Starten Sie in jedem der drei Tabs eine Transaktion, indem Sie «BEGIN TRANSACTION;» eingeben und auf «Execute» klicken.

Führen Sie die Befehle aus der Tabelle  erneut aus.

Was fällt Ihnen auf?

Mit Serializable warten die Transaktionen, bis eine abgeschlossen ist, wodurch alle Probleme (Dirty Reads, Non-repeatable Reads) verhindert werden. Bei Read Committed treten keine Dirty Reads auf, aber Non-repeatable Reads und Datenüberschreiben sind möglich. Serializable bietet maximale Sicherheit, Read Committed bessere Performance.