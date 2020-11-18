﻿using Melanchall.DryWetMidi.Core;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MuseL
{
    public class MuseReciever : IDisposable
    {
        private Task recieveTask;
        private CancellationTokenSource cts;

        public static string[] Genres = new string[]
        {
            "chopin","mozart","rachmaninoff","ladygaga","country","disney","jazz","bach","beethoven","journey","thebeatles","video","broadway","franksinatra","bluegrass","tchaikovsky","liszt","everything","ragtime","andrehazes","cocciante","thecranberries","ligabue","metallica","traffic","philcollins","nineinchnails","thepretenders","sugarray","grandfunkrailroad","ron","ellington","fleetwoodmac","thebeachboys","kool & the gang","foreigner","tlc","scottjames","benfoldsfive","smashmouth","oasis","allsaints","donnasummer","weezer","bjork","mariahcarey","berte","cheaptrick","caroleking","thecars","gganderson","robertpalmer","zucchero","alicecooper","vanhalen","brucehornsby","coolio","jimmybuffett","lobo","badcompany","eminem","creedenceclearwaterrevival","deeppurple","shearinggeorge","robbiewilliams","dalla","ub40","lindaronstadt","sinatra","inxs","jonimitchell","michaeljackson","last","devo","shaniatwain","korn","brooksgarth","sweet","thewho","roxette",
            "bowiedavid","beegees","renefroger","mina","estefangloria","mccartney","theventures","carboni","simplyred","santana","jewel","meatloaf","giorgia","nofx","rickymartin","thecure","thetemptations","tozzi","beck","eiffel65","jenniferlopez","reelbigfish","patsycline","richardcliff","styx","acdc","brucespringsteen","michaelgeorge","blondie","pinkfloyd","oldfieldmike","redhotchilipeppers","therollingstones","morandi","heart","robertaflack","pantera","alabama","jethrotull","hanson","mosch","ludwigvanbeethoven","dvorak","chrisrea","guns n' roses","duranduran","ericclapton","bettemidler","bwitched","gordonlightfoot","thegrassroots","chicago","whitezombie","michaelbolton","paulsimon","marillion","thepointersisters","theanimals","cher","haydn","aerosmith","supertramp","littleriverband","america","tonyorlando","tompetty","thecorrs","aliceinchains","kiss","prince","toto","vanmorrison","wagner","cashjohnny","annielennox","enya","thedoobiebrothers","thetragicallyhip",
            "rush","laurapausini","stevemillerband","simonandgarfunkel","fiorellamannoia","olivianewton-john","carlysimon","elvispresley","vangelis","bobdylan","bbking","vengaboys","paoli","thehollies","alainsouchon","pooh","raf","fiorello","lionelrichie","jimihendrix","theeverlybrothers","limpbizkit","donhenley","georgeharrison","threedognight","johnmellencamp","carpenters","raycharles","basie","billyocean","scorpions","royorbison","whitneyhouston","ironmaiden","jovanotti","alanjackson","barrymanilow","hueylewis","kennyloggins","chopinfrederic","talkingheads","themonkees","rem","jeanmicheljarre","michelezarrillo","eurythmics","thedoors","guesswho","miller","thefourseasons","matiabazar","tompettyandtheheartbreakers","chickcorea","scottjoplin","amedeominghi","bryanadams","paulaabdul","rossivasco","billyjoel","daniele","claudedebussy","gilbert & sullivan","chakakhan","nirvana","garbage","andreabocelli","johnnyrivers","emerson, lake & palmer","theallmanbrothersband",
            "zappa","boston","mango","barbrastreisand","willsmith","ozzyosbourne","janetjackson","antonellovenditti","u2","humperdinckengelbert","jamiroquai","zero","chuckberry","spicegirls","ledzeppelin","masini","thekinks","eagles","billyidol","alanismorissette","joecocker","jimcroce","bobmarley","blacksabbath","stonetemplepilots","silverchair","paulmccartney","blur","nek","greenday","thepolice","depechemode","rageagainstthemachine","madonna","rogerskenny","brooks & dunn","883","thedrifters","amygrant","herman","toriamos","eltonjohn","britneyspears","lennykravitz","celentano","ringostarr","neildiamond","aqua","oscarpeterson","joejackson","moby","collinsphil","leosayer","takethat","electriclightorchestra","pearljam","marcanthony","borodin","petshopboys","stevienicks","hollybuddy","turnertina","annaoxa","zztop","sting","themoodyblues","ruggeri","creed","claudebolling","renzoarbore","erasure","elviscostello","airsupply","tinaturner","leali","petergabriel","nodoubt",
            "bread","huey lewis & the news","brandy","level42","radiohead","georgebenson","wonderstevie","thesmashingpumpkins","cyndilauper","rodstewart","bush","ramazzotti","bobseger","theshadows","gershwin","cream","biagioantonacci","steviewonder","nomadi","direstraits","davidbowie","amostori","thealanparsonsproject","johnlennon","crosbystillsnashandyoung","battiato","kansas","clementi","richielionel","yes","brassensgeorges","steelydan","jacksonmichael","buddyholly","earthwindandfire","natkingcole","therascals","bonjovi","alanparsons","backstreetboys","glencampbell","howardcarpendale","thesupremes","villagepeople","blink-182","jacksonbrowne","sade","lynyrdskynyrd","foofighters","2unlimited","battisti","hall & oates","stansfieldlisa","genesis","boyzone","theoffspring","tomjones","davematthewsband","johnelton","neilyoung","dionnewarwick","aceofbase","marilynmanson","taylorjames","rkelly","grandi","sublime","edvardgrieg","tool","bachjohannsebastian","patbenatar","celinedion","queen","soundgarden","abba","drdre","defleppard","dominofats","realmccoy","natalieimbruglia","hole","spinners","arethafranklin","reospeedwagon","indian","movie","scottish","irish","african","taylorswift","shakira","blues","latin","katyperry","world","kpop","africandrum","michaelbuble","rihanna","gospel","beyonce","chinese","arabic","adele","kellyclarkson","theeagles","handel","rachmaninov","schumann","christmas","dance","punk","natl_anthem","brahms","rap","ravel","burgmueller","other","schubert","granados","albeniz","mendelssohn","debussy","grieg","moszkowski","godowsky","folk","mussorgsky","kids","balakirev","hymns","verdi","hummel","deleted","delibes","saint-saens","puccini","satie","offenbach","widor","songs","stravinsky","vivaldi","gurlitt","alkan","weber","strauss","traditional","rossini","mahler","soler","sousa","telemann","busoni","scarlatti","stamitz","classical","jstrauss2","gabrieli","nielsen","purcell","donizetti","kuhlau","gounod","gibbons","weiss","faure","holst","spohr","monteverdi","reger","bizet","elgar","czerny","sullivan","shostakovich","franck","rubinstein","albrechtsberger","paganini","diabelli","gottschalk","wieniawski","lully","morley","sibelius","scriabin","heller","thalberg","dowland","carulli","pachelbel","sor","marcello","ketterer","rimsky-korsakov","ascher","bruckner","janequin","anonymous","kreutzer","sanz","joplin","susato","giuliani","lassus","palestrina","smetana","berlioz","couperin","gomolka","daquin","herz","campion","walthew","pergolesi","reicha","polak","holborne","hassler","corelli","cato","azzaiolo","anerio","gastoldi","goudimel","dussek","prez","cimarosa","byrd","praetorius","rameau","khachaturian","machaut","gade","perosi","gorzanis","smith","haberbier","carr","marais","glazunov","guerrero","cabanilles","losy","roman","hasse","sammartini","blow","zipoli","duvernoy","aguado","cherubini","victoria","field","andersen","poulenc","d'aragona","lemire","krakowa","maier","rimini","encina","banchieri","best","galilei","warhorse","gypsy","soundtrack","encore","roblaidlow","nationalanthems","benjyshelton","ongcmu","crosbystillsnashyoung","smashingpumpkins","aaaaaaaaaaa","alanismorrisette","animenz","onedirection","nintendo","disneythemes","gunsnroses","rollingstones","juliancasablancas","abdelmoinealfa","berckmansdeoliveira","moviethemes","beachboys","davemathews","videogamethemes","moabberckmansdeoliveira","unknown","cameronleesimpson","johannsebastianbach","thecarpenters","elo","nightwish","blink182","emersonlakeandpalmer","tvthemes"

        };

        private string[] encodedMidi;

        public string[] EncodedMidi
        {
            get
            {
                lock (threadLock)
                {
                    return (string[])encodedMidi.Clone();
                }
            }
        }

        private readonly string genre;
        private readonly Instruments instruments;
        private readonly int temperature;
        private readonly int trunication;

        public readonly int totalDurration;

        public int Tokens { get
            {
                lock (threadLock)
                {
                    return encodedMidi.Length;
                }
            } }
        private int currentDurration = 0;
        public int CurrentDurration
        {
            get
            {
                lock (threadLock)
                {
                    return currentDurration;
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return recieveTask!=null&&!recieveTask.IsCanceled && !recieveTask.IsFaulted && !recieveTask.IsCompleted;
            }
        }

        private object threadLock = new object();

        public MuseReciever(MidiFile startMelodie, string genre, Instruments instruments, int temperature, int turnication, float length)
        {
            if (startMelodie != null)
                encodedMidi = MuseEncoder.Encode(startMelodie);
            else
                encodedMidi = new string[0];

            currentDurration = MuseDecoder.GetDurration(encodedMidi);

            this.genre = genre;
            this.instruments = instruments;
            this.temperature = temperature;
            this.trunication = turnication;
            this.totalDurration = (int) (length * 60 * 1000);
        }

        public string GetName()
        {
            return "F-"+genre;
        }

        public MuseReciever Clone()
        {
            return (MuseReciever)this.MemberwiseClone();
        }

        public void ActivateRecieving()
        {
            if (recieveTask != null)
                return;

            cts = new CancellationTokenSource();
            recieveTask = Recieve(cts.Token);
            //DebugTask();
        }

        private void DebugTask()
        {
            Debug.Log("RecieveTask: IsCompleted: " + recieveTask.IsCompleted +
                " isCanceled: " + recieveTask.IsCanceled +
                " IsFaulted: " + recieveTask.IsFaulted);
        }

        public async Task Recieve(CancellationToken cancellation)
        {
            int durration;
            string[] encoding;

            lock (threadLock)
            {
                encoding = (string[])encodedMidi.Clone();
                durration = currentDurration;
            }

            while(durration < totalDurration)
            {
                if (cancellation.IsCancellationRequested)
                    return;

                encoding = await MuseNetworkRequest.MakeMuseRequest(encoding, genre, instruments, temperature, trunication, cancellation); 
                durration = MuseDecoder.GetDurration(encoding);

                lock (threadLock)
                {
                    encodedMidi = encoding;
                    currentDurration = durration;
                }
            }
        }

        

        public void Play()
        {
            //Debug.Log("Play L: "+encodedMidi.Length);
            //DebugTask();
            MidiPlayer.Play(MuseDecoder.DecodeMuseEncoding(EncodedMidi));
        }

        public void Dispose()
        {
            cts?.Cancel();
        }
    }

    //public enum Genre
    //{
    //    chopin,mozart,rachmaninoff,ladygaga,country,disney,jazz,bach,beethoven,journey,thebeatles,video,broadway,franksinatra,bluegrass,tchaikovsky,liszt,everything,ragtime,andrehazes,cocciante,thecranberries,ligabue,metallica,traffic,philcollins,nineinchnails,thepretenders,sugarray,grandfunkrailroad,ron,ellington,fleetwoodmac,thebeachboys,kool & the gang,foreigner,tlc,scottjames,benfoldsfive,smashmouth,oasis,allsaints,donnasummer,weezer,bjork,mariahcarey,berte,cheaptrick,caroleking,thecars,gganderson,robertpalmer,zucchero,alicecooper,vanhalen,brucehornsby,coolio,jimmybuffett,lobo,badcompany,eminem,creedenceclearwaterrevival,deeppurple,shearinggeorge,robbiewilliams,dalla,ub40,lindaronstadt,sinatra,inxs,jonimitchell,michaeljackson,last,devo,shaniatwain,korn,brooksgarth,sweet,thewho,roxette,bowiedavid,beegees,renefroger,mina,estefangloria,mccartney,theventures,carboni,simplyred,santana,jewel,meatloaf,giorgia,nofx,rickymartin,thecure,thetemptations,tozzi,beck,eiffel65,jenniferlopez,reelbigfish,patsycline,richardcliff,styx,acdc,brucespringsteen,michaelgeorge,blondie,pinkfloyd,oldfieldmike,redhotchilipeppers,therollingstones,morandi,heart,robertaflack,pantera,alabama,jethrotull,hanson,mosch,ludwigvanbeethoven,dvorak,chrisrea,gunsroses,duranduran,ericclapton,bettemidler,bwitched,gordonlightfoot,thegrassroots,chicago,whitezombie,michaelbolton,paulsimon,marillion,thepointersisters,theanimals,cher,haydn,aerosmith,supertramp,littleriverband,america,tonyorlando,tompetty,thecorrs,aliceinchains,kiss,prince,toto,vanmorrison,wagner,cashjohnny,annielennox,enya,thedoobiebrothers,thetragicallyhip,rush,laurapausini,stevemillerband,simonandgarfunkel,fiorellamannoia,olivianewtonjohn,carlysimon,elvispresley,vangelis,bobdylan,bbking,vengaboys,paoli,thehollies,alainsouchon,pooh,raf,fiorello,lionelrichie,jimihendrix,theeverlybrothers,limpbizkit,donhenley,georgeharrison,threedognight,johnmellencamp,carpenters,raycharles,basie,billyocean,scorpions,royorbison,whitneyhouston,ironmaiden,jovanotti,alanjackson,barrymanilow,hueylewis,kennyloggins,chopinfrederic,talkingheads,themonkees,rem,jeanmicheljarre,michelezarrillo,eurythmics,thedoors,guesswho,miller,thefourseasons,matiabazar,tompettyandtheheartbreakers,chickcorea,scottjoplin,amedeominghi,bryanadams,paulaabdul,rossivasco,billyjoel,daniele,claudedebussy,gilbertsullivan,chakakhan,nirvana,garbage,andreabocelli,johnnyrivers,emerson, lake & palmer,theallmanbrothersband,zappa,boston,mango,barbrastreisand,willsmith,ozzyosbourne,janetjackson,antonellovenditti,u2,humperdinckengelbert,jamiroquai,zero,chuckberry,spicegirls,ledzeppelin,masini,thekinks,eagles,billyidol,alanismorissette,joecocker,jimcroce,bobmarley,blacksabbath,stonetemplepilots,silverchair,paulmccartney,blur,nek,greenday,thepolice,depechemode,rageagainstthemachine,madonna,rogerskenny,brooks & dunn,883,thedrifters,amygrant,herman,toriamos,eltonjohn,britneyspears,lennykravitz,celentano,ringostarr,neildiamond,aqua,oscarpeterson,joejackson,moby,collinsphil,leosayer,takethat,electriclightorchestra,pearljam,marcanthony,borodin,petshopboys,stevienicks,hollybuddy,turnertina,annaoxa,zztop,sting,themoodyblues,ruggeri,creed,claudebolling,renzoarbore,erasure,elviscostello,airsupply,tinaturner,leali,petergabriel,nodoubt,bread,huey lewis & the news,brandy,level42,radiohead,georgebenson,wonderstevie,thesmashingpumpkins,cyndilauper,rodstewart,bush,ramazzotti,bobseger,theshadows,gershwin,cream,biagioantonacci,steviewonder,nomadi,direstraits,davidbowie,amostori,thealanparsonsproject,johnlennon,crosbystillsnashandyoung,battiato,kansas,clementi,richielionel,yes,brassensgeorges,steelydan,jacksonmichael,buddyholly,earthwindandfire,natkingcole,therascals,bonjovi,alanparsons,backstreetboys,glencampbell,howardcarpendale,thesupremes,villagepeople,blink-182,jacksonbrowne,sade,lynyrdskynyrd,foofighters,2unlimited,battisti,hall & oates,stansfieldlisa,genesis,boyzone,theoffspring,tomjones,davematthewsband,johnelton,neilyoung,dionnewarwick,aceofbase,marilynmanson,taylorjames,rkelly,grandi,sublime,edvardgrieg,tool,bachjohannsebastian,patbenatar,celinedion,queen,soundgarden,abba,drdre,defleppard,dominofats,realmccoy,natalieimbruglia,hole,spinners,arethafranklin,reospeedwagon,indian,movie,scottish,irish,african,taylorswift,shakira,blues,latin,katyperry,world,kpop,africandrum,michaelbuble,rihanna,gospel,beyonce,chinese,arabic,adele,kellyclarkson,theeagles,handel,rachmaninov,schumann,christmas,dance,punk,natl_anthem,brahms,rap,ravel,burgmueller,other,schubert,granados,albeniz,mendelssohn,debussy,grieg,moszkowski,godowsky,folk,mussorgsky,kids,balakirev,hymns,verdi,hummel,deleted,delibes,saint-saens,puccini,satie,offenbach,widor,songs,stravinsky,vivaldi,gurlitt,alkan,weber,strauss,traditional,rossini,mahler,soler,sousa,telemann,busoni,scarlatti,stamitz,classical,jstrauss2,gabrieli,nielsen,purcell,donizetti,kuhlau,gounod,gibbons,weiss,faure,holst,spohr,monteverdi,reger,bizet,elgar,czerny,sullivan,shostakovich,franck,rubinstein,albrechtsberger,paganini,diabelli,gottschalk,wieniawski,lully,morley,sibelius,scriabin,heller,thalberg,dowland,carulli,pachelbel,sor,marcello,ketterer,rimsky-korsakov,ascher,bruckner,janequin,anonymous,kreutzer,sanz,joplin,susato,giuliani,lassus,palestrina,smetana,berlioz,couperin,gomolka,daquin,herz,campion,walthew,pergolesi,reicha,polak,holborne,hassler,corelli,cato,azzaiolo,anerio,gastoldi,goudimel,dussek,prez,cimarosa,byrd,praetorius,rameau,khachaturian,machaut,gade,perosi,gorzanis,smith,haberbier,carr,marais,glazunov,guerrero,cabanilles,losy,roman,hasse,sammartini,blow,zipoli,duvernoy,aguado,cherubini,victoria,field,andersen,poulenc,d'aragona,lemire,krakowa,maier,rimini,encina,banchieri,best,galilei,warhorse,gypsy,soundtrack,encore,roblaidlow,nationalanthems,benjyshelton,ongcmu,crosbystillsnashyoung,smashingpumpkins,aaaaaaaaaaa,alanismorrisette,animenz,onedirection,nintendo,disneythemes,gunsnroses,rollingstones,juliancasablancas,abdelmoinealfa,berckmansdeoliveira,moviethemes,beachboys,davemathews,videogamethemes,moabberckmansdeoliveira,unknown,cameronleesimpson,johannsebastianbach,thecarpenters,elo,nightwish,blink182,emersonlakeandpalmer,tvthemes
    //}

    [Flags]
    public enum Instruments
    {
        piano=1,strings=2,winds=4,drums=8,harp=16,guitar=32,bass=64
    }
}
