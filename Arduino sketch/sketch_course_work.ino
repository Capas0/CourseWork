#define trigger 170 // 10101010 - признак начала передачи
#define ping 125
#define led 12
#define photoPin A0
#define flagPin 13
#define edge 100

int u = 0;
int oldu = 0;
byte now = 0;
int on = 0;
int pointer = 0;

int oldus[5]; // Аналог oldu для Shift5
byte logs[5]; // Аналог now для Shift5
int flags[5]; // Аналог on для Shift5

void Send(byte symbol);
void Shift();
void Shift5();

void setup()
{
  Serial.begin(9600);
  pinMode(led, OUTPUT);
  pinMode(flagPin, OUTPUT);
  pinMode(photoPin, INPUT);

  digitalWrite(flagPin, HIGH);
  oldu = analogRead(photoPin);
  for (int i = 0; i < 5; i++)
  {
    oldus[i] = oldu;
    logs[i] = 0;
    flags[i] = 0;
  }
}

void loop()
{
  if (Serial.available() == 0)
  {
    Shift5();
    if (now == trigger)
    {
      delay(ping * 4 / 5);
      Send(trigger); //Передача признака начала приема
      digitalWrite(flagPin, LOW);
      for (int i = 0; i < 15; i++) // Прием 15 байт
        Serial.write(Take());
      for (int i = 0; i < 5; i++)
        logs[i] = 0;
    }
  }
  else
  {
    do
    {
      Send(trigger);// передача признака начала передачи
      for (int i = 0; i < 16 * 5 && now != trigger; i++)
      {
        Shift5();
      }
    } while (now != trigger);
    digitalWrite(flagPin, LOW);
    for (int i = 0; i < 15; i++) // передача 15 байт
    {
      if (Serial.available() > 0)
      {
        Send(Serial.read());
      }
      else
        delay(8 * ping);
    }
    now = 0;
    for (int i = 0; i < 5; i++)
      logs[i] = 0;
    digitalWrite(led, LOW);
  }
  digitalWrite(flagPin, HIGH);
}

void Shift()
{
  u = analogRead(photoPin);
  if (u - oldu > edge)
    on = 1;
  else if (oldu - u > edge)
    on = 0;
  now <<= 1;
  now += on;
  oldu = u;
  delay(ping);
}

void Shift5()
{
  u = analogRead(photoPin);
  if (u - oldus[pointer] > edge)
    flags[pointer] = 1;
  else if (oldus[pointer] - u > edge)
    flags[pointer] = 0;
  logs[pointer] <<= 1;
  logs[pointer] += flags[pointer];
  oldus[pointer] = u;
  now = logs[pointer];
  pointer = (pointer + 1) % 5;
  delay(ping / 5);
}

void Send(byte symbol)
{
  for (int j = 0; j < 8; j++)
  {
    if ((symbol & (1 << (7 - j))) > 0)
      digitalWrite(led, HIGH);
    else
      digitalWrite(led, LOW);
    delay(ping);
  }
}

byte Take()
{
  now = 0;
  for (int j = 0; j < 8; j++)
    Shift();
  return now;
}
