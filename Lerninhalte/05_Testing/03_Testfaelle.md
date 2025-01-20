# Testfälle

Gerade für nicht erwartete Probleme hilft es, menschliche Intelligenz zur Seite zu haben. Das heisst, es braucht Testfälle und Tests für kompetente Tester:innen.

Nehmen Sie an, es existieren folgende Anforderungen:

1. Nur Benutzer:innen in der Rolle «Administrators» oder «Users» können alle Ledgers sehen.
2. Bei einer Buchung wird ein Geldbetrag von einem Konto auf ein anderes verschoben.
3. Nur Benutzer:innen in der Rolle «Administrators» können Ledgers verändern.
4. Bei einer Buchung darf der Kontostand nie unter 0 fallen.
5. ...

Gehen Sie davon aus, dass Ihre Applikation nach den Anforderungen entwickelt wurde und keine Features fehlen.


## Aufgabenstellung

Sie sollen für die Softwarequalitätssicherung Testfälle erstellen.

### Teilaufgabe 1: Übersicht

Sie beschliessen, als Erstes einmal kurz aufzuschreiben, was Sie alles testen lassen möchten.

Machen Sie eine Liste in der Form:

- Testfall 1: Buchung in der Rolle «Administrators»
- Testfall 2: Buchung in der Rolle "User"
- Testfall 3: Buchung mit negativem Kontostand
- Testfall 4: Zugriff auf Ledgers durch Benutzer mit Rolle «Administrators»
- Testfall 5: Zugriff auf Ledgers durch Benutzer mit Rolle «User»
- Testfall 6: Zugriff auf Ledgers durch Benutzer ohne entsprechende Rollen
- Testfall 7: Veränderung eines Ledgers durch Benutzer mit der Rolle «Administrators»
- Testfall 8: Veränderung eines Ledgers durch Benutzer mit der Rolle «Users»
- Testfall 9: Gleichzeitiger Zugriff und Buchung

Tipps:

- Jeder Testfall hat genau eine Anforderung, die er möglichst spezifisch testen soll. Das ist manchmal schwierig
- Jede Anforderung muss von mindestens einem, kann aber von mehreren Testfällen getestet werden.
- Es soll sich immer um ein beobachtbares Verhalten handeln.
- Sie haben keinen Zugriff auf den Quellcode oder die Entwicklungsumgebung.
- Testen Sie auch was nicht möglich sein darf.
- Testen Sie auch potenzielle Probleme bei der Datenintegrität bei gleichzeitigem Zugriff.
- Sie sollten mindestens 6 sinnvolle Testfälle finden, 9 ist realistischer.

Welche Tests eignen sich besser als explorative Testfälle, weil sie keine klaren ein- und Ausgaben haben?
1. Testen von Nebenbedingungen bei gleichzeitiger Nutzung:
        Beobachten, ob unvorhergesehene Probleme bei gleichzeitigen Buchungen auftreten (z. B. Deadlocks, Inkonsistenzen).
2. Fehlerhafte oder ungewöhnliche Eingaben bei Buchungen:
        Eingabe leerer, extrem großer oder negativer Werte testen, um zu sehen, wie robust die Anwendung ist.



### Teilaufgabe 2: Testfälle
Formulieren Sie die Testfälle korrekt aus, also mit Testfallnummer, getestete Anforderung, konkreter Eingabe und konkreter Ausgabe.

Testfall 1: Buchung in der Rolle «Administrators»               
Getestete Anforderung: Benutzer:innen mit der Rolle „Administrators“ können erfolgreich Buchungen durchführen.          
Eingabe: Ein Benutzer mit der Rolle "Administrators" bucht 100 € von Konto A (Kontostand: 200 €) auf Konto B (Kontostand: 300 €).               
Erwartete Ausgabe: Der Kontostand von Konto A sinkt auf 100 €, der Kontostand von Konto B steigt auf 400 €. Eine Erfolgsmeldung wird angezeigt.

Testfall 2: Buchung in der Rolle «Users»                
Getestete Anforderung: Benutzer:innen mit der Rolle „Users“ können erfolgreich Buchungen durchführen.           
Eingabe: Ein Benutzer mit der Rolle "Users" bucht 50 € von Konto X (Kontostand: 200 €) auf Konto Y (Kontostand: 100 €).         
Erwartete Ausgabe: Der Kontostand von Konto X sinkt auf 150 €, der Kontostand von Konto Y steigt auf 150 €. Eine Erfolgsmeldung wird angezeigt.

Testfall 3: Buchung mit negativem Kontostand            
Getestete Anforderung: Der Kontostand darf bei einer Buchung nicht unter 0 fallen.              
Eingabe: Ein Benutzer mit der Rolle "Administrators" versucht, 300 € von Konto A (Kontostand: 200 €) auf Konto B zu buchen.             
Erwartete Ausgabe: Die Buchung wird abgelehnt, eine Fehlermeldung („Kontostand darf nicht negativ sein“) wird angezeigt, und die Kontostände bleiben unverändert.

Testfall 4: Zugriff auf Ledgers durch Benutzer mit Rolle «Administrators»               
Getestete Anforderung: Benutzer:innen mit der Rolle "Administrators" können alle Ledgers sehen.         
Eingabe: Ein Benutzer mit der Rolle "Administrators" ruft die Liste der Ledgers ab.             
Erwartete Ausgabe: Eine vollständige Liste der Ledgers wird angezeigt (z. B. Ledger1, Ledger2, Ledger3).

Testfall 5: Zugriff auf Ledgers durch Benutzer mit Rolle «Users»                
Getestete Anforderung: Benutzer:innen mit der Rolle "Users" können ebenfalls alle Ledgers sehen.                
Eingabe: Ein Benutzer mit der Rolle "Users" ruft die Liste der Ledgers ab.              
Erwartete Ausgabe: Eine vollständige Liste der Ledgers wird angezeigt (z. B. Ledger1, Ledger2, Ledger3).

Testfall 6: Zugriff auf Ledgers durch Benutzer ohne entsprechende Rollen                
Getestete Anforderung: Benutzer:innen ohne die Rollen "Administrators" oder "Users" dürfen keine Ledgers sehen.         
Eingabe: Ein Benutzer ohne eine der genannten Rollen ruft die Liste der Ledgers ab.             
Erwartete Ausgabe: Der Zugriff wird verweigert, und eine Fehlermeldung („Zugriff verweigert“) wird angezeigt.

Testfall 7: Veränderung eines Ledgers durch Benutzer mit der Rolle «Administrators»             
Getestete Anforderung: Benutzer:innen mit der Rolle "Administrators" können Ledgers verändern.          
Eingabe: Ein Benutzer mit der Rolle "Administrators" ändert den Namen eines Ledgers von „Ledger1“ auf „LedgerA“.                
Erwartete Ausgabe: Der Name des Ledgers wird in „LedgerA“ geändert, und eine Erfolgsmeldung wird angezeigt.

Testfall 8: Veränderung eines Ledgers durch Benutzer mit der Rolle «Users»              
Getestete Anforderung: Benutzer:innen mit der Rolle "Users" dürfen Ledgers nicht verändern.             
Eingabe: Ein Benutzer mit der Rolle "Users" versucht, den Namen eines Ledgers von „Ledger1“ auf „LedgerA“ zu ändern.            
Erwartete Ausgabe: Die Änderung wird abgelehnt, eine Fehlermeldung („Zugriff verweigert“) wird angezeigt, und der Ledger-Name bleibt unverändert.

Testfall 9: Gleichzeitiger Zugriff und Buchung  
Getestete Anforderung: Die Datenintegrität bleibt bei gleichzeitigem Zugriff aufrechterhalten.  
Eingabe: Zwei Benutzer (Benutzer A und Benutzer B) versuchen gleichzeitig, jeweils 100 € von Konto Z (Kontostand: 150 €) auf unterschiedliche Konten zu buchen.         
Erwartete Ausgabe: Nur eine Buchung wird erfolgreich ausgeführt (Kontostand von Konto Z sinkt auf 50 €). Die zweite Buchung wird abgelehnt, und eine Fehlermeldung wird angezeigt.

### Teilaufgabe 3: Exploratives Testen
Schreiben Sie eine Kurze aber klare Anleitung für das explorative Testen in der Form:
- Zu testende Anforderungen
- Rahmenbedingungen: Auf was soll geachtet werden
- Testideen


- testende Anforderungen
   1. Benutzer:innen mit den Rollen "Administrators" und "Users" können alle Ledgers einsehen.
   2. Nur Benutzer:innen mit der Rolle "Administrators" können Ledgers verändern.
   3. Eine Buchung darf nicht ausgeführt werden, wenn der Kontostand des Quellkontos dadurch unter 0 fällt.
   4. Gleichzeitige Buchungen durch mehrere Benutzer:innen dürfen keine Dateninkonsistenzen erzeugen.

- Rahmenbedingungen
   - Systemstatus: Das Backend ist verfügbar, und die Testumgebung ist mit gültigen Testdaten eingerichtet.
   - Rollen: Benutzer:innen mit verschiedenen Rollen ("Administrators", "Users", keine Rolle) sind vorhanden.
   - Zugriffsrechte: Unterschiedliche Berechtigungen für die Rollen müssen aktiv überprüft werden.
   - Reaktionszeit: Beobachten, wie das System bei hoher Last oder unerwartetem Verhalten reagiert.
   - Fehlerbehandlung: Achten Sie darauf, ob Fehlermeldungen sinnvoll und informativ sind.


- Testidee


1. Zugriffsrechte prüfen:             
   - Melden Sie sich mit verschiedenen Benutzerrollen an und versuchen Sie:
      - Alle Ledgers anzuzeigen.
      - Ledgers zu verändern.
      - Auf APIs oder Funktionen zuzugreifen, die für andere Rollen gesperrt sein sollten.
   - Beobachten Sie, ob unerlaubter Zugriff korrekt abgelehnt wird.


2. Ungewöhnliche Buchungsszenarien:
   - Versuchen Sie Buchungen mit extremen Werten (z. B. 0 €, negative Beträge, sehr hohe Beträge).
   - Testen Sie Buchungen mit leeren oder ungültigen Feldern.
   - Beobachten Sie, ob das System konsistente Fehlermeldungen liefert.


3. Gleichzeitiger Zugriff:
   - Starten Sie mehrere parallele Buchungsvorgänge, die denselben Kontostand verändern könnten.
   - Beispiel: Zwei Benutzer:innen versuchen, jeweils 200 € von einem Konto mit 300 € Kontostand zu buchen.
   - Überprüfen Sie, ob eine Buchung abgelehnt wird und ob der Kontostand korrekt bleibt.


4. Fehlerhafte API-Kommunikation:
   - Simulieren Sie Netzwerkprobleme, um zu prüfen, wie das System auf unterbrochene oder verzögerte API-Aufrufe reagiert.
   - Prüfen Sie, ob Anfragen sauber abgebrochen oder erneut gesendet werden.


5. Manipulation von Daten:
   - Testen Sie, ob Eingaben von Benutzer:innen (z. B. JSON-Anfragen an die API) manipuliert werden können, um unerlaubte Aktionen auszuführen (z. B. Änderung von Ledgers durch "Users").
   - Überprüfen Sie die Validierung und Sanitisierung der Eingaben.


6. Skalierbarkeit und Performance:
   - Testen Sie, wie das System bei einer Vielzahl paralleler Buchungen reagiert.
   - Achten Sie auf Verzögerungen oder Fehler unter hoher Last.


7. Benutzererfahrung:
   - Beobachten Sie, ob Fehlermeldungen klar und verständlich sind.
   - Stellen Sie sicher, dass das Verhalten des Systems den Erwartungen der Benutzer:innen entspricht (z. B. intuitive Navigation, korrekte Feedbackmeldungen).