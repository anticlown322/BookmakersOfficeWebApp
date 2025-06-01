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
import { RoleGuard } from './core/services/user-service/role.guard';
import { Role } from './core/models/shared/enums/role.enum';
import { UserListComponent } from './pages/user-list/user-list.component';
import { BetListComponent } from './pages/bet-list/bet-list.component';
import { PayoutListComponent } from './pages/payout-list/payout-list.component';
import { EmailConfirmationComponent } from './pages/email-confirmation/email-confirmation.component';

export const routes: Routes = [
    { path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'teams', component: TeamsComponent },
    { path: 'prematch', component: PrematchComponent },
    { path: 'results', component: ResultsPageComponent },
    { path: 'email-confirmation/:username', component: EmailConfirmationComponent },
    {
        path: 'profile',
        component: ProfileComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Administrator, Role.Gambler, Role.Moderator] },
    },
    {
        path: 'balance',
        component: BalanceComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Gambler] },
    },
    {
        path: 'my-bets',
        component: UserBetsComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Gambler] },
    },
    {
        path: 'user-list',
        component: UserListComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Administrator] },
    },
    {
        path: 'bet-list',
        component: BetListComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Administrator] },
    },
    {
        path: 'payout-list',
        component: PayoutListComponent,
        canActivate: [RoleGuard],
        data: { roles: [Role.Administrator] },
    },
];
