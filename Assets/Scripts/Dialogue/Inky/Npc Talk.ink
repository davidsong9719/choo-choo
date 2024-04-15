-> Talk
===Talk===
{shuffle:
- Hello #speaker-Player
    {shuffle:
        - Hi? #speaker-Opponent
          ->hello
            
        - Do I know you? #speaker-Opponent
            ->hello
    }
- Join our cult #speaker-Player
    {shuffle:
        - No thanks #speaker-Opponent
            ->hello
        - Convince me #speaker-Opponent
            ->hello
    }
- Plz we need money #speaker-Player
    {shuffle:
        - I do too #speaker-Opponent
            ->hello
        - Don't we all? #speaker-Opponent
            ->hello
    }
}

-> END


===hello===
* hi #speaker-Player
    bye #speaker-Opponent
* im so awesome #speaker-Player
    keep dreaming #speaker-Opponent
- ->END
