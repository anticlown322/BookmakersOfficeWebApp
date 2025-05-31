import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { LoginComponent } from './pages/login/login.component';
import { RegisterComponent } from './pages/register/register.component';
import { TeamsComponent } from './pages/teams/teams.component';
import { ResultsPageComponent } from './pages/results/results.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { BalanceComponent } from './pages/balance/balance.component';
import { PrematchComponent } from './pages/prematch/prematch.component';
import { UserBetsComponent } from './pages/user-bets/user-bets.component';

export const routes: Routes = [
  { path: '', component: HomeComponent, pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'teams', component: TeamsComponent },  
  { path: 'prematch', component: PrematchComponent },
  { path: 'results', component: ResultsPageComponent },
  { path: 'profile', component: ProfileComponent },
  { path: 'balance', component: BalanceComponent },
  { path: 'my-bets', component: UserBetsComponent },
];