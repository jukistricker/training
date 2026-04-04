namespace HomeController {
    export function Initial(): void{
        console.log("HomeController Initial");
        let isDarKMode: boolean = false;
        const incBtn = document.getElementById("clickButton");
        const counterText = document.getElementById("counter");
        const resetBtn = document.getElementById("resetButton");
        const themeBtn = document.getElementById("switch-theme");
        incBtn?.addEventListener("click", OnAction);
        resetBtn?.addEventListener("click", OnReset);
        themeBtn?.addEventListener("click", SwitchTheme);

        function OnAction(): void{
            if(counterText){
                let count = parseInt(counterText.innerText);
                count++;
                counterText.innerText = count.toString();
            }
        }

        function OnReset(): void{
            if(counterText){
                counterText.innerText = "0";
            }   
        }

        function SwitchTheme(): void{
            const body = document.body;
            if(isDarKMode){
                body.classList.remove("dark-theme");
            } else {
                body.classList.add("dark-theme");
            }
        }


    }
}