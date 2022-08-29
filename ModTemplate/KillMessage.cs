using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtifactLaser
{
    public enum KillMessage
    {
        ROASTED,
        REDUCEDTOATOMS,
        ANNIHILATED,
        KICKED,
        BADTIME,
        GAMEOVER,
        REKT,
        BUSTED,
        MEDIUMWELL
    }

    public static class KillMessageConverter
    {
        private static Random rand = new Random();

        /**
         * Gives the string associated with a given killmessage type. The given name is automatically inserted into the string
         */
        public static string getMessage(KillMessage msgType, string name)
        {
            string msg;

            //Big old switch to determine what the message should be
            switch(msgType)
            {
                case KillMessage.ROASTED:
                    msg = name + " just got roasted!";
                    break;
                case KillMessage.REDUCEDTOATOMS:
                    msg = name + " was reduced to atoms!";
                    break;
                case KillMessage.ANNIHILATED:
                    msg = name + " was annihilated!";
                    break;
                case KillMessage.KICKED:
                    msg = name + " just got kicked!";
                    break;
                case KillMessage.BADTIME:
                    msg = name + " had a bad time!";
                    break;
                case KillMessage.MEDIUMWELL:
                    msg = name + " is now medium-well!";
                    break;
                case KillMessage.GAMEOVER:
                    msg = "It's game over for " + name + "!";
                    break;
                case KillMessage.REKT:
                    msg = name + " just got rekt!";
                    break;
                case KillMessage.BUSTED:
                    msg = name + " just got busted!";
                    break;
                default:
                    msg = name + " just died!";
                    break;
            }

            return msg;
        }

        /**
         * Gets a random killmessage string, using the given name
         */
        public static string getRandomMessage(string name)
        {
            //Pick a random message type
            KillMessage[] msgList = Enum.GetValues(typeof(KillMessage)) as KillMessage[];
            int index = rand.Next(msgList.Length);

            //Return the string for it
            return getMessage(msgList[index], name);
        }
    }
}
