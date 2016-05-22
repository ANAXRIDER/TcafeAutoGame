using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcafeAutoGame
{
    class JavaScripts
    {
        private string prefixAttentionGame;
        private string postfixAttentionGame;
        private string typingGame;

        public JavaScripts()
        {
            prefixAttentionGame = "javascript: " +
                                        "(function()" +
                                        "{" +
                                            "var cbInt = 0;" +
                                            "cbInt = window.setInterval(function()" +
                                            "{" +
                                                "if (counter > 0)" +
                                                "{" +
                                                    "window.clearInterval(cbInt);" +
                                                    "cbInt = window.setInterval(function()" +
                                                    "{" +
                                                        "counter = ";

            postfixAttentionGame =                          ";" +
                                                        "document.getElementById('btn_stop').click();" +
                                                        "window.clearInterval(cbInt);" +
                                                    "}, 1);" +
                                                "}" +
                                                "else" +
                                                "{" +
                                                    "document.getElementById('btn_start').click();" +
                                                    "this.total_count = 0;" +
                                                "}" +
                                            "}, 100);" +
                                        "} )();";

            typingGame = "javascript: " +
                                            "(function()" +
                                            "{" +
                                                "document.getElementById('htp_In_str').value = document.getElementById('tzStr').innerHTML + ' ';" +
                                                "http.timerSec = Math.floor((http.tzStr.length+1) * 5000 / (700));" +
                                                "http.keyUp();" +
                                            "} )();";
        }

        public string GetAttentionGameScript(int goal)
        {
            return prefixAttentionGame + goal + postfixAttentionGame;
        }

        public string GetTypingGameScript()
        {
            return typingGame;
        }
    }
}
