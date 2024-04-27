VAR lastScore = 15
VAR highScore = 20
VAR score = 0
VAR tutorialStage = 1

->Ending
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
        - We were fated to meet and I was fated to show you the truth. #speaker-Player
    {shuffle:
        - Prove it. #speaker-Opponent
        - I don't believe in that. #speaker-Opponent
            You will soon. #speaker-Player
        - Fated to insult me? #speaker-Opponent
            Merely opening your eyes. #speaker-Player
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


===Win===
I did it! #speaker-Player
Im convinced. #speaker-Opponent
->END

===Lose===
Nah, im good. #speaker-Opponent
->END







===Tutorial===
Ah our newest member. You know about clicking [E] to continue, yes? #speaker-Opponent
Wonderful, have you been briefed already? #speaker-Opponent
A briefing is highly reccomended!
    *[No (tutorial)]
        ~ tutorialStage = 2
        Not yet! What are my tasks? #speaker-Player
        Eager, are you? Those above would be proud. #speaker-Opponent
        //The higher beings have been growing hungry as of late.
        //And we've run low on sacrifices, so you have a special job of gathering them!
        The higher beings have been feeling upset, too few are like you and I.
        Too few have opened their eyes to the truth.
        It is our job as loyal servants to bring salvation to the rest of the world.
        Save them from the ignorance and darkness they've been lost in.
        You've been assigned the subway tunnels.
        Find anyone you can and talk to them, open their eyes.
        You can use [E] to interact with people and objects as well.
        Come back and interact with me when you are ready for the practice recruitment.
        
    *[Yes (skip tutorial)]
    ~ tutorialStage = 4
        Yes! I have and I'm ready to serve! #speaker-Player
        Impressive. Go to the left to find a station and get started then. #speaker-Opponent
        I'll return at 9:00pm. 
- -> END

===Tutorial2===
Welcome back. #speaker-Opponent
Did you get a chance to explore?
*[Yes]
    Yeah! Very awesome! #speaker-Player
    Good, you will be very familiar with these stations soon. #speaker-Opponent
*No... #speaker-Player
    Worry not, there will be more chances. #speaker-Opponent
- Now, let us begin. #speaker-Opponent
  In a few moments, we'll have a mock recruitment.
  You see the bar underneath me? That's my Willpower.
  Yours is up there, on the top left.
  You need to lower the lost souls's Willpower enough to get them to join our cu-
  Our church.
  The little bar on the side is for speed, this determines our talking order.
  To use a card, pull it up until its eyes open.
  Let's give it a try.
-> END

===Tutorial3===
You picked your tactics, good job. #speaker-Opponent
And look at that, you've gained me as a follower. 
You can check your follower count up in the top right corner.
Now, head over to the stairs to the left and wait for a train.
You can check where you are on your map on the top left.
Recruit as many followers as you can by the end of the day, I'll see you at 9:00 PM.
Good luck, soldier.
-> END

===TutorialWin===
Did I do it? #speaker-Player
You've worn down my Willpower, congratulations. You learn fast. #speaker-Opponent
After every recruitment you'll have the opportunity to learn new tactics.
You can learn up to three and a minimum of one.
Keep in mind sometimes you forget what you learned.
Just click on the tactic you want to learn and the one you want to replace and confirm.
->END
===TutorialLose===
How. #speaker-Opponent
->END




===Ending===
You're back. #speaker-Opponent
How many followers have you recruited?
{score}!
{score > lastScore: 
    Impressive. 
    {score > highScore: 
        Incredibly impressive, actually.
        I think thats the highest amount of followers I've ever heard recruited in day.
        I only got {lastScore}.
        Those above shine down upon you, little one.
        You are blessed.
    - else:
        That's even more than me, you learn fast. 
        I only got {lastScore}.
    }
- else:
	Good job, you're learning quick.
	Maybe you'll even beat me soon.
	I got {lastScore}{score == lastScore: too}.
}
{score <= highScore: 
    I've heard rumors of a fellow worshipper from the other district.
    Apparrently they've {score == highScore: also} recruited {highScore} new followers.
    That's the highest recorded by those above.
    {score == highScore: You're well on your way to beating their record.}
}
->END
