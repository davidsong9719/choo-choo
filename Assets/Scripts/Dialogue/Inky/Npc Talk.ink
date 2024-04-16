-> Talk
===Talk===
{shuffle:
    - Hello! #speaker-Player
    {shuffle:
        - Hi? #speaker-Opponent
            -> hello.option1
        - Do I know you? #speaker-Opponent
            -> hello.option2
        - Sorry, have we met? #speaker-Opponent
            -> hello.option3
        - Hello? #speaker-Opponent
            -> hello.option1
    }
    
    - You look sad. #speaker-Player
    And lonely. #speaker-Player
    And pathetic. #speaker-Player
    {shuffle:
        - Me? #speaker-Opponent
            -> pathetic.option1
        - Excuse me? #speaker-Opponent
            -> pathetic.option2
        - How dare you? #speaker-Opponent
            -> pathetic.option3
        - You talking about me? #speaker-Opponent
            -> pathetic.option1
    }
    
    - Could I have a moment of your time? #speaker-Player
    {shuffle:
        - Can't you see I'm busy? #speaker-Opponent
            ->time.option1
        - What for? #speaker-Opponent
            ->time.option2
    }
}
-> END


===hello===
= option1
    *Hi! #speaker-Player
    *Nice to meet you! #speaker-Player
- -> hello.continue

= option2
    * [You will soon.]
        Not yet, but you will soon. #speaker-Player
    * [I know you.]
        No, but I know you. #speaker-Player
    - -> hello.continue
    
= option3
* [In another life]
    Not here, but in another life we may have. #speaker-Player
    {shuffle:
        - What's that supposed to mean? #speaker-Opponent
        - How would you know? #speaker-Opponent
    }
    
* We have now! #speaker-Player
    Just as predicted. Like clockwork. #speaker-Player
    {shuffle:
        - That's creepy. #speaker-Opponent
        - What are you? Some stalker? #speaker-Opponent
    }
- -> hello.continue

= continue
Would you like one of these pamplets? #speaker-Player
    - {shuffle:
        - I'm good. #speaker-Opponent
            {shuffle:
                - You are not good until you've read it! #speaker-Player
                - We can make you better than good! #speaker-Player
            }
        - No, thanks. #speaker-Opponent
            I insist! #speaker-Player
       }
-> END



===pathetic===
= option1
    * Obviously. Look at you. #speaker-Player
    * Who else? #speaker-Player
- ->pathetic.continue

= option2
    * [I said...]
        I said you look sad, lonely and pathetic. #speaker-Player
        {shuffle:
            - Gee. Thanks. #speaker-Opponent
            - Who gave you the right to say that? #speaker-Opponent
            - You're doing wonders for my self-esteem right now. #speaker-Opponent
            - You do this to everyone you meet? #speaker-Opponent
        }
    * Excuse you. #speaker-Player
        {shuffle:
            - How original. #speaker-Opponent
            - What's the point of this? #speaker-Opponent
            - What is it you want? #speaker-Opponent
        }
    - Being aware of your flaws is the first step to recovery and the truth! #speaker-Player
-> pathetic.continue

= option3
    * Look at it positively! #speaker-Player
        We are here now. #speaker-Player
        And we see the way to fulfilling your true potential. #speaker-Player
    * We all started like you. #speaker-Player
        They helped us see the truth. #speaker-Player
        Join us, and you can see too. #speaker-Player
- -> pathetic.continue

= continue
    * They sent me to help you. #speaker-Player
    * This is inevitable.  #speaker-Player
        - It's destiny, you see? #speaker-Player
        We were fated to meet and I was fated to show you the truth. #speaker-Player
    {shuffle:
        - Prove it. #speaker-Opponent
        - I don't believe in that. #speaker-Opponent
            You will soon. #speaker-Player
        - Fated to insult me? #speaker-Opponent
            Merely opening your eyes. #speaker-Player
            Allow me to explain. #speaker-Player
    }
-> END

===time===
= option1
    *It'll only be a moment. #speaker-Player
        {shuffle:
            - Make it quick. #speaker-Opponent
            - It better be. #speaker-Opponent
            - Whatever it is, I'm not interested. #speaker-Opponent
        }
    *You'll want to hear this. #speaker-Player
        {shuffle:
            - I doubt it. #speaker-Opponent
            - I want to leave. #speaker-Opponent
            - I'm certain I won't. #speaker-Opponent
        }
    - ->time.continue
= option2
    *The opportunity of your dreams. #speaker-Player
        {shuffle:
            - My dreams were crushed long ago. #speaker-Opponent
            - Okay? You've peaked my curiousity. #speaker-Opponent
            - That's a big promise. #speaker-Opponent
        }
    *Infinite knowledge. #speaker-Player
        {shuffle:
            - That's impossible. #speaker-Opponent
            - Now I know you're lying. #speaker-Opponent
            - Is this a joke? #speaker-Opponent
        }
    - -> time.continue
= continue
    We provide a new way to live life, to look at the world. #speaker-Player
    I'm not convinced. #speaker-Opponent
    And immortality. #speaker-Player
    Okay, I'm more convinced. #speaker-Opponent
->END
