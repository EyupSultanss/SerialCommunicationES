const int PIN_D2 = 2;
const int PIN_D3 = 3;
const int PIN_D4 = 4;

void setup() {
  Serial.begin(115200);
  pinMode(PIN_D2, OUTPUT);
  pinMode(PIN_D3, OUTPUT);
  pinMode(PIN_D4, OUTPUT);
  digitalWrite(PIN_D2, LOW);
  digitalWrite(PIN_D3, LOW);
  digitalWrite(PIN_D4, LOW);
}

void loop() {
  if (Serial.available() > 0) {
    String line = Serial.readStringUntil('\n');
    line.trim();
    line.toLowerCase();
    if (line.startsWith("set ")) {
      // expected: set dX high|low
      int firstSpace = line.indexOf(' ');
      int secondSpace = line.indexOf(' ', firstSpace + 1);
      if (firstSpace >= 0 && secondSpace > firstSpace) {
        String pinToken = line.substring(firstSpace + 1, secondSpace); // e.g., "d3"
        String valueToken = line.substring(secondSpace + 1); // e.g., "high"
        int pin = -1;
        if (pinToken == "d2") pin = PIN_D2;
        else if (pinToken == "d3") pin = PIN_D3;
        else if (pinToken == "d4") pin = PIN_D4;

        if (pin != -1) {
          if (valueToken == "high" || valueToken == "on" || valueToken == "1") {
            digitalWrite(pin, HIGH);
            Serial.println("OK");
          } else if (valueToken == "low" || valueToken == "off" || valueToken == "0") {
            digitalWrite(pin, LOW);
            Serial.println("OK");
          } else {
            Serial.println("ERR unknown value");
          }
        } else {
          Serial.println("ERR unknown pin");
        }
      } else {
        Serial.println("ERR format");
      }
    } else {
      Serial.println("ERR unknown command");
    }
  }
}
