using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Text;

namespace AppForCalc.Controllers
{
    public class ComputeController : Controller 
    {
        //
        // GET: /Compute/

        public ActionResult Index(string x)
        {
            var calculator = new PostfixNotationExpression();
            ViewBag.Message = calculator.result(x.Replace(" ", "+"));

            return View();
        }
    }


    class PostfixNotationExpression
    {
        public PostfixNotationExpression()
        {
            operators = new List<string>(standart_operators);

        }
        List<string> operators;
        List<string> standart_operators =
            new List<string>(new string[] { "(", ")", "+", "-", "*", "/" });

        IEnumerable<string> Separate(string input)
        {
            int pos = 0;
            while (pos < input.Length)
            {
                string s = string.Empty + input[pos];
                
                if(s=="-" && (pos==0 || !Char.IsDigit(input[pos-1])))
                {
                    for (int i = pos + 1; i < input.Length &&
                       (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                        s += input[i];
                }
                else if (!standart_operators.Contains(input[pos].ToString()) && Char.IsDigit(input[pos]))
                {
                    for (int i = pos + 1; i < input.Length &&
                        (Char.IsDigit(input[i]) || input[i] == ',' || input[i] == '.'); i++)
                        s += input[i];
                }

                yield return s;
                pos += s.Length;
            }
        }

        byte GetPriority(string s)
        {
            switch (s)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 3;
            }
        }

        public string[] ConvertToPostfixNotation(string input)
        {
            List<string> outputSeparated = new List<string>();
            Stack<string> stack = new Stack<string>();
            foreach (string c in Separate(input))
            {
                if (operators.Contains(c))
                {
                    if (stack.Count > 0 && !c.Equals("("))
                    {
                        if (c.Equals(")"))
                        {
                            string s = stack.Pop();
                            while (s != "(")
                            {
                                outputSeparated.Add(s);
                                s = stack.Pop();
                            }
                        }
                        else if (GetPriority(c) > GetPriority(stack.Peek()))
                            stack.Push(c);
                        else
                        {
                            while (stack.Count > 0 && GetPriority(c) <= GetPriority(stack.Peek()))
                                outputSeparated.Add(stack.Pop());
                            stack.Push(c);
                        }
                    }
                    else
                        stack.Push(c);
                }
                else
                    outputSeparated.Add(c);
            }
            if (stack.Count > 0)
                foreach (string c in stack)
                    outputSeparated.Add(c);

            return outputSeparated.ToArray();
        }

        public decimal result(string input)
        {
            Stack<string> stack = new Stack<string>();
            Queue<string> queue = new Queue<string>(ConvertToPostfixNotation(input));
            string str = queue.Dequeue();
            while (queue.Count >= 0)
            {
                if (!operators.Contains(str))
                {
                    stack.Push(str);
                    str = queue.Dequeue();
                }
                else
                {
                    decimal summ = 0;
                    try
                    {

                        switch (str)
                        {

                            case "+":
                                {
                                    decimal a = Convert.ToDecimal(stack.Pop());
                                    decimal b = Convert.ToDecimal(stack.Pop());
                                    summ = a + b;
                                    break;
                                }
                            case "-":
                                {
                                    decimal a = Convert.ToDecimal(stack.Pop());
                                    decimal b = Convert.ToDecimal(stack.Pop());
                                    summ = b - a;
                                    break;
                                }
                            case "*":
                                {
                                    decimal a = Convert.ToDecimal(stack.Pop());
                                    decimal b = Convert.ToDecimal(stack.Pop());
                                    summ = b * a;
                                    break;
                                }
                            case "/":
                                {
                                    decimal a = Convert.ToDecimal(stack.Pop());
                                    decimal b = Convert.ToDecimal(stack.Pop());
                                    summ = b / a;
                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    stack.Push(summ.ToString());
                    if (queue.Count > 0)
                        str = queue.Dequeue();
                    else
                        break;
                }

            }
            return Convert.ToDecimal(stack.Pop());
        }
    }
}
