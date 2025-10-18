DOKUMENT: PRODUKTIONSPLAN & ROADMAP - RE:WIND
​Version: 1.0
Datum: 2025-10-16
Syfte: Att definiera teamets roller, ansvar, tidslinje och milstolpar för utvecklingen av RE:WIND, med ett specifikt fokus på en AI-assisterad utvecklingspipeline.
​1. Team och Roller: En AI-augmenterad Struktur

**Spelmotor:** Unity har valts som spelmotor för detta projekt. Detta beslut baseras på teamets befintliga erfarenhet av motorn, vilket kommer att påskynda utvecklingen och minska inlärningskurvan.

​I denna produktionsmodell agerar människor som arkitekter, regissörer och validerare, medan LLM-agenterna fungerar som exekutörer för väldefinierade kodningsuppgifter. Rollerna skiftar från "skapare av kod" till "specifikerare och granskare av kod".
​Lead Designer / Game Director (Människa):
​Ansvar: Håller i den övergripande visionen. Är den ytterst ansvariga för att GDD:t är kristallklart, konsekvent och detaljerat nog för att kunna översättas till prompts för kod-agenterna. Sätter de kreativa målen för varje milstolpe.
​Technical Director / Systemarkitekt (Människa):
​Ansvar: Den absolut viktigaste tekniska rollen. Designar den övergripande systemarkitekturen (hur TimeManager, AnchorManager, WorldState etc. ska interagera). Skapar de tekniska blueprints och API-kontrakt som agenterna ska följa. Granskar agenternas kod för att säkerställa att den passar in i helheten och uppfyller prestandakrav.
​AI-assisterad Utvecklare / Systemintegratör (Människa, utveckling av programmerarrollen):
​Ansvar: Denna roll är en hybrid av programmerare och prompt engineer.
​Specifikationsskrivning: Bryter ner GDD:ts funktioner till exakta, tekniska prompts som en LLM-agent kan exekvera. Exempel: "Skriv en TimeManager-klass i C# för Unity som implementerar följande metoder från vårt GDD: StartLoop(), EndLoop(reason), Update(deltaTime). Klassen ska broadcasta LoopStartEvent och LoopEndEvent via vår EventBus."
​Granskning & Validering: Granskar och testar koden som agenten genererar. Säkerställer att den är buggfri, effektiv och följer projektets kodstandard.
​Integration & Felsökning: Integrerar de agent-genererade modulerna med varandra. Felsöker komplexa problem som uppstår mellan systemen, vilket är något agenterna är sämre på.
​Komplex Problemlösning: Kodar manuellt de mest komplexa och nyskapande delarna av spelet (t.ex. paradox-logiken, tidslinjemanipuleringen) där agenternas förmåga inte räcker till.
​3D-Artist / Ljuddesigner / Narrativ Designer (Människa):
​Ansvar: Deras roller är mer traditionella, men de måste leverera tillgångar (assets) som är tydligt namngivna och strukturerade så att utvecklarna enkelt kan instruera agenterna att implementera dem. Exempel: "Implementera ljudeffekten SFX_Loop_Reset.wav när LoopEndEvent anropas."
​QA-Testare (Människa):
​Ansvar: Får en ännu viktigare roll. Måste inte bara hitta buggar, utan också identifiera ologiskt eller ineffektivt beteende i den AI-genererade koden. Deras feedback är avgörande för att förfina prompts och lära upp agenterna.
​LLM Coding Agents (Verktyg):
​Roll: Exekverar väldefinierade kodningsuppgifter. Skriver klasser, funktioner, UI-element och enklare system baserat på de mänskliga utvecklarnas specifikationer. Kan även användas för att generera unit-tester och dokumentation.
​2. Tidslinje och Milstolpar
​Denna tidslinje antar ett litet, effektivt team (ca 3-5 personer). Tidsvinsten med agenter ligger främst i prototyp- och funktionsimplementeringsfaserna.
​Fas 1: Pre-produktion & Prototyping (2 månader)
​Mål: Validera kärnloopen och de viktigaste spelmekanikerna. Bevisa att den AI-assisterade pipeline fungerar.
​Mänskligt Fokus: Systemarkitekten designar grunden. Lead Designer och Utvecklare skriver extremt detaljerade prompts för kärnsystemen.
​Agent-Fokus: Genererar snabbt flera olika versioner av TimeManager, spelarkontrollen, ett grundläggande pusselsystem och KnowledgeManager. Teamet kan testa olika loop-längder och mekaniker utan att lägga veckor på manuell kodning.
​Resultat: En spelbar prototyp med fungerande 7-minutersloop, ett pussel och ett interaktionssystem.
​Fas 2: Vertical Slice (3 månader)
​Mål: Producera Kapitel 1 till nära slutgiltig kvalitet. Alla kärnsystem ska vara implementerade och fungera tillsammans.
​Mänskligt Fokus: Integrera, polera och felsöka systemen från Fas 1. Konstnärer och ljuddesigners skapar de slutgiltiga tillgångarna för Kapitel 1. Utvecklare skriver prompts för AI-patruller, UI (klockan, anslagstavlan) och AnchorManager.
​Agent-Fokus: Implementerar de fullständiga versionerna av kärnsystemen enligt arkitektens blueprint. Bygger UI-elementen och skriptar de specifika pusslen i Kapitel 1 baserat på designerns logik.
​Resultat: En fullt spelbar, polerad version av Kapitel 1 som representerar den slutgiltiga spelupplevelsen.
​Fas 3: Alfa - "Content Complete" (6 månader)
​Mål: Allt innehåll (Kapitel 2-5) är implementerat och spelbart från början till slut. System för tidsdistortion, paradoxer och tidslinjemanipulering är på plats.
​Mänskligt Fokus: Designa de komplexa pusslen i senare kapitel. Arkitekten övervakar integrationen av de avancerade temporala systemen. Utvecklare fokuserar på att skriva prompts för innehåll (nya pussel, fiendemönster) och manuellt koda de svåraste paradox-systemen.
​Agent-Fokus: Massproduktion av innehåll. Agenterna skriptar majoriteten av pusslen, AI-beteenden och händelser i Kapitel 2-5 baserat på de mallar och system som skapades i Vertical Slice.
​Resultat: Hela spelet är spelbart, men buggigt, opolerat och obalanserat.
​Fas 4: Beta - "Feature Freeze & Polishing" (3 månader)
​Mål: Inga nya funktioner läggs till. Fokus ligger 100% på buggfixning, optimering, balansering och polering.
​Mänskligt Fokus: Dominerar denna fas. Utvecklare och QA-testare identifierar och fixar buggar. Mycket av detta är komplext felsökningsarbete mellan olika AI-genererade system. Lead Designer balanserar pusslens svårighetsgrad.
​Agent-Fokus: Används för väldefinierade optimeringsuppgifter ("Refaktorera denna funktion för att minska minnesanvändning") och att åtgärda enklare, isolerade buggar.
​Resultat: Spelet är stabilt, prestandaoptimerat och innehållsmässigt komplett.
​Fas 5: Gold Master & Lansering (1 månad)
​Mål: Slutföra de sista kritiska buggfixarna och förbereda för lansering på valda plattformar.
​Mänskligt Fokus: Nästan uteslutande mänskligt arbete. Hantering av plattformsspecifika krav (certifiering för PlayStation/Xbox), marknadsföring och community-förberedelser.
​Resultat: En färdig version av spelet redo att publiceras.