VAR lastScore = 5
VAR highScore = 5
VAR score = 0
VAR tutorialStage = 1

->Tutorial
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
        - What? #speaker-Opponent
    }
    
* We have now! #speaker-Player
    Just as predicted. Like clockwork. #speaker-Player
    {shuffle:
        - That's creepy. #speaker-Opponent
        - What are you? Some stalker? #speaker-Opponent
        - Leave me alone. #speaker-Opponent
    }
- -> hello.continue

= continue
Would you like one of these pamplets? #speaker-Player
    - {shuffle:
        - I'm good. #speaker-Opponent
            {shuffle:
                - You are not good until you've read it! #speaker-Player
                - We can make you better than good! #speaker-Player
                - Have one anyway!
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
But it's okay! #speaker-Player
    * Look at it positively! #speaker-Player
        We are here now. #speaker-Player
        And we see the way to fulfilling your true potential. #speaker-Player
    * We all started like you. #speaker-Player
        They helped us see the truth. #speaker-Player
- -> pathetic.continue

= continue
    Join us for dinner tomorrow, we can help you. #speaker-Player
    {shuffle:
        - How? #speaker-Opponent
        - I don't need your help. #speaker-Opponent
            But you need theirs. #speaker-Player
        - I have dinner plans. #speaker-Opponent
            None that'll change your life like this! #speaker-Player
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
    We can make you.. technically immortal! #speaker-Player
    I'm not convinced. #speaker-Opponent
    We have free food? #speaker-Player
    Okay, I'm more convinced. #speaker-Opponent
->END


===Win===
{shuffle:
    - So? Pamphlet? #speaker-Player
    Yeah yeah, whatever. #speaker-Opponent
    I'll take your stupid pamphlet.
    Awesome! #speaker-Player
    
    - You know... This doesn't sound too bad... #speaker-Opponent
    You said you provided free food right?
    Plenty. #speaker-Player
    
    - Alright, alright. I'll think about it. #speaker-Opponent
    That's all I need! #speaker-Player
    
    - You really should go. Just visit tomorrow! #speaker-Player
    I'll go if I have time. #speaker-Opponent
    You won't regret it! #speaker-Player
    
    - Come on, we're hosting a big dinner tomorrow! #speaker-Player
    The Higher Beings will be there too! 
    I suppose I am a bit tired of cooking... #speaker-Opponent
}
->END

===Lose===
{shuffle:
    - AAAAAAAAAAAAA #speaker-Opponent
    What the?? #speaker-Player
    AAAAAAAAAAAA #speaker-Opponent
    What are you doing? #speaker-Player
    AAAAAAAAAA #speaker-Opponent
    Alright, alright I'm going! Quit screaming! #speaker-Player
    AAAAAAA #speaker-Opponent
    
    - Sorry, this is my stop. #speaker-Opponent
    Wait! Take a pamphlet before you go! #speaker-Player
    Sorry, I'm in a rush! Bye! #speaker-Opponent
    
    - Your cult isn't as incredible as you seem to believe. #speaker-Opponent
    It's not a cult! #speaker-Player
    And it's awesome.
    Not awesome enough for me. Goodbye. #speaker-Opponent
    
    - There's something else to this. #speaker-Opponent
    I don't trust you. #speaker-Player
    But I'm so trustworthy! #speaker-Player
    This is too suspicious, I'm reporting you. #speaker-Opponent
    No, wait! I'll go, I'll go! #speaker-Player
}
->END







===Tutorial===
Ah our newest member. You know about <b> clicking [E] to continue</b>, yes? #speaker-Opponent
Yep! #speaker-Player
Wonderful, do you need me to go over your assignment? #speaker-Opponent
A briefing is highly recommended!
    *[Yes (tutorial)]
        ~ tutorialStage = 2
        Yes, please! What are my tasks? #speaker-Player
        Eager, are you? Those above would be proud. #speaker-Opponent
        The Higher Beings have been growing hungry as of late.
        And we've run low on sacrifices.
        I think I see where this is going! #speaker-Player
        Good. It is our job as mere servants to feed the Perceptive Ones. #speaker-Opponent
        Thank them for watching over us below.
        You've been assigned to the subway tunnels.
        Go talk to people there and <b>recruit them as loyal followers</b> of the Higher.
        Remember not to tell them about the feeding.
        Mortals are not as... open to such things as we. 
        They don't see what we do.
        Of course! Silly creatures. #speaker-Player
        You can use <b>[E] to interact</b> with people. You can do the same to objects. #speaker-Opponent
        Come back and interact with me when you are ready for the practice recruitment.
        
    *[No (skip tutorial)]
    ~ tutorialStage = 4
        Nope! I'm all caught up and ready to serve! #speaker-Player
        Impressive. Go to the left to find a station and get started then. #speaker-Opponent
        I'll return at <b>8:00 PM</b>. 
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
  You see the bar underneath me? That's my <b>Willpower</b>.
  Yours is up there, on the top left.
  You need to <b>lower the lost souls's Willpower</b> enough to get them to join our cu-
  Our <i>church</i>.
  The little bar on the side is for <b>speed</b>, this determines our talking order.
  When you reach the bottom of the bar, it's your turn.
  To use a card, <b>pull it up until its eyes open</b>.
  Let's give it a try.
-> END

===Tutorial3===
You picked your tactics, good job. #speaker-Opponent
And look at that, you've <b>gained me as a follower</b>. 
You can check your <b>follower count</b> up in the top right corner.
You can also check where you are on your <b>map</b> on the top left.
There, you'll see which station you're on and where to switch lines.
The <b><color=\#8929A2>Pulse Line</color></b> contains a lot of angry people, you'll learn plenty of aggressive tactics there.
The <b><color=\#3C51C3>Gallium Line</color></b> has more defensive people, they'd be helpful with protective tactics.
The <b><color=\#218A86>Pilgrim Line</color></b> is filled with other clever souls. Great for wildcard tactics.
Now, head over to the <b>stairs to the left</b> and wait for a train.
Recruit as many followers as you can by the end of the day, I'll see you at <b>8:00 PM</b>.
Oh and, before you go, <b>head to the statue to regain your Willpower</b>.
There's one in every station.
Good luck, soldier.
-> END

===TutorialWin===
Did I do it? #speaker-Player
You've worn down my Willpower, congratulations. You learn fast. #speaker-Opponent
As a quick rundown, <b>arguments will deplete your opponent's Willpower</b>.
<b>Retorts will recover your own</b>.
But keep in mind that <b>you can only retort your opponent's last argue</b>.
Retorting any other time won't aid you.
There are various other actions you can take called "<b>effects</b>".
You can learn these when you learn new tactics.
You'll have the opportunity to learn new tactics after every successful recruitment.
You can learn <b>up to three</b> and a minimum of one.
Keep in mind sometimes you forget what you used to know.
Just click on the tactic you want to learn and the one to replace and confirm.
->END
===TutorialLose===
How. #speaker-Opponent
This is coded to be impossible.
->END




===Ending===
You're back. #speaker-Opponent
Have fun talking to people? 
*[Yes!]
Best experience of my life! #speaker-Player
*[Yes!]
Nothing has been more fun than this! #speaker-Player
- Wonderful. #speaker-Opponent
How many followers have you recruited?
{score}!#speaker-Player
{score > lastScore: 
    Impressive. #speaker-Opponent
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
	Good job, you're learning quick. #speaker-Opponent
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