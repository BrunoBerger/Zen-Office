from prodict import Prodict

Demographic = Prodict(
    Keys = [
        "DD01_01",
        "DD02",
        "DD03",
        "DD04_01",
        "DD04_02",
        "DD05",
        "DD06",
        "DD07",
        "DD08_01",
        "DD09_01",
        "DD10",
    ],
    Names = [
        "Alter",
        "ArErfahrung",
        "Arbeitsumgebung",
        "Bildschirmzeit im Beruf/Studium",
        "Bildschirmzeit in der Freizeit",
        "Cybersickness",
        "NaturZurEntspannung",
        "NutzungRelaxApps",
        "WennJaWelcheApps",
        "UserID",
        "Geschlecht"
    ]
)

SSSQ = Prodict(
    Scale = Prodict(
        Names = [
            "Unzufrieden",
            "Aufmerksam",
            "Deprimiert",
            "Traurig",
            "Aktiv",
            "Ungeduldig",
            "Genervt",
            "Wütend",
            "Irritiert",
            "Mürrisch",
        ],
        AR = Prodict(
            Listen_Keys = [
                "SS01_01",
                "SS01_02",
                "SS01_03",
                "SS01_04",
                "SS01_05",
                "SS01_06",
                "SS01_07",
                "SS01_08",
                "SS01_09",
                "SS01_10",
            ],
            Sudoku_Keys = [
                "SS06_01",
                "SS06_02",
                "SS06_03",
                "SS06_04",
                "SS06_05",
                "SS06_06",
                "SS06_07",
                "SS06_08",
                "SS06_09",
                "SS06_10",
            ],
        ),
        NonAR = Prodict(
            Listen_Keys = [
                "SS03_01",
                "SS03_02",
                "SS03_03",
                "SS03_04",
                "SS03_05",
                "SS03_06",
                "SS03_07",
                "SS03_08",
                "SS03_09",
                "SS03_10",
            ],
            Sudoku_Keys = [
                "SS08_01",
                "SS08_02",
                "SS08_03",
                "SS08_04",
                "SS08_05",
                "SS08_06",
                "SS08_07",
                "SS08_08",
                "SS08_09",
                "SS08_10",
            ]
        )
    ),
    StatementAgreement = Prodict(
        Names = [
            "Ich war entschlossen meine Leistungsziele zu erreichen",
            "Ich wollte bei der Aufgabe erfolgreich sein",
            "Ich war motiviert die Aufgabe umzusetzen",
            "Ich führte die Aufgabe kompetent aus",
            "Grundsätzlich fühlte ich, dass ich die Dinge unter Kontrolle hatte",
        ],
        Names_e = [
            "I was committed to attaining my performance goals",
            "I wanted to be succeed on the task",
            "I was motivated to do the task",
            "I performed proficiently on this task",
            "Generally, I felt in control of things",
        ],
        AR = Prodict(
            Listen_Keys = [
                "SS02_01",
                "SS02_02",
                "SS02_03",
                "SS02_11",
                "SS02_12",
            ],
            Sudoku_Keys = [
                "SS07_01",
                "SS07_02",
                "SS07_03",
                "SS07_11",
                "SS07_12",
            ],
        ),
        NonAR = Prodict(
            Listen_Keys = [
                "SS04_01",
                "SS04_02",
                "SS04_03",
                "SS04_11",
                "SS04_12",
            ],
            Sudoku_Keys = [
                "SS09_01",
                "SS09_02",
                "SS09_03",
                "SS09_11",
                "SS09_12",
            ],
        )
    )
)

ShortUserExpQ = Prodict(
    Names_left = [
        "behindernd",
        "kompliziert",
        "ineffizient",
        "verwirrend",
        "langweilig",
        "uninteressant",
        "konventionell", 
        "herkömmlich",
    ],
    Names_right = [
        "unterstützend",
        "einfach",
        "effizient",
        "übersichtlich",
        "spannend",
        "interessant",
        "originell",
        "neuartig",
    ],
    Keys = [
        "UE01_01",
        "UE01_02",
        "UE01_03",
        "UE01_04",
        "UE01_05",
        "UE01_06",
        "UE01_07",
        "UE01_08",
    ]
)

AbschließendeFragen = Prodict(
    Keys = [
        "CF01_01",
        "CF01_02",
        "CF01_03",
        "CF01_04",
        "CF01_05",
        "CF01_06",
        "CF01_07",
        "CF01_08",
        "CF01_09",
        "CF01_10",
        "CF01_11",
    ],
    Names = [
        "Ich war grundsätzlich aufgeregt, eine Augmented Reality Projektion zu erleben.",
        "Ich empfand Übelkeit durch die virtuelle Projektionstechnik (Cyber Sickness)",
        "Der eingeschränkte Sichtbereich der Projektion störte die Immersion",
        "Das Tragen des Geräts war unangenehm",
        "Ich hatte das Gefühl mich in der virtuellen Umgebung zu befinden",
        "Ich habe die virtuelle Umgebung auch während dem Erledigen der Aufgaben beachtet",
        "Die virtuelle Umgebung hat meine Konzentration gestört",
        "Durch die virtuelle Umgebung habe ich mich naturnäher gefühlt",
        "Durch die virtuelle Umgebung fühlte ich mich entspannter",
        "Ich könnte mir vorstellen diese Anwendung zum Arbeiten zu nutzen",
        "Ich könnte mir vorstellen eine solche Anwendung in Zukunft zu nutzen, wenn bestimmte Aspekte verbessert werden",
    ]
)

EmotionScales = Prodict(
    Names = [
        "Freude",
        "Trauer",
        "Angst",
        "Überraschung",
        "Wut",
        "Ekel",
    ],
    AR = Prodict(
        Vorher = [
            "EM01_01",
            "EM01_02",
            "EM01_03",
            "EM01_04",
            "EM01_05",
            "EM01_06",
        ],
        Zwischen = [
            "EM02_01",
            "EM02_02",
            "EM02_03",
            "EM02_04",
            "EM02_05",
            "EM02_06",
        ],
        Nachher = [
            "EM03_01",
            "EM03_02",
            "EM03_03",
            "EM03_04",
            "EM03_05",
            "EM03_06",
        ]
    ),
    NonAR = Prodict(
        Vorher = [
            "EM04_01",
            "EM04_02",
            "EM04_03",
            "EM04_04",
            "EM04_05",
            "EM04_06",
        ],
        Zwischen = [
            "EM05_01",
            "EM05_02",
            "EM05_03",
            "EM05_04",
            "EM05_05",
            "EM05_06",
        ],
        Nachher = [
            "EM06_01",
            "EM06_02",
            "EM06_03",
            "EM06_04",
            "EM06_05",
            "EM06_06",
        ]
    )
)

if __name__ == "__main__":
    print("Testing dics")
    try:
        Demographic.Names
        SSSQ.Scale.AR.Listen_Keys
        ShortUserExpQ.Names_left
        AbschließendeFragen.Keys
        EmotionScales.AR.Zwischen
    except KeyError as e:
        print("Error in one of the dictionaries:", e)
        exit(1)
    print("No errors")