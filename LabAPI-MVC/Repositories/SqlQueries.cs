namespace LabAPI_MVC.Repositories;

public static class SqlQueries
{
    public const string GetReferralBase = """
                                              select r.referral_id,
                                                     r.patient_id,
                                                     r.issued,
                                                     r.weight,
                                                     r.height,
                                                     r.sex
                                                from prelab.referral r
                                               where r.referral_id = @id
                                          """;
    public const string GetReferralTests = """
                                              select rt.test_id,
                                                     t.name,
                                                     t.description,
                                                     b.biomaterial_id,
                                                     b.name,
                                                     b.description
                                                from prelab.referral_test rt
                                                join prelab.test t
                                                  on rt.test_id = t.test_id
                                                join prelab.biomaterial b
                                                  on b.biomaterial_id = t.biomaterial_id
                                               where rt.referral_id = @id
                                           """;

    public const string GetPatientBase = """
                                                 select p.patient_id,
                                                        p.full_name,
                                                        p.birth_date,
                                                        p.document,
                                                        p.phone,
                                                        p.email
                                                   from prelab.patient p
                                                  where p.patient_id = @id
                                         """;
    public const string GetReferralSamples = """
                                                select s.sample_id,
                                                       s.issued,
                                                       bc.bio_case_id,
                                                       bc.name,
                                                       bc.description,
                                                       bcn.bio_container_id,
                                                       bcn.name,
                                                       bcn.description,
                                                       b.biomaterial_id,
                                                       b.name,
                                                       b.description,
                                                       sp.supplier_id,
                                                       sp.name,
                                                       sp.description
                                                  from prelab.sample s
                                                  join prelab.bio_case bc
                                                    on bc.bio_case_id = s.bio_case_id
                                                  join prelab.bio_container bcn
                                                    on bcn.bio_container_id = bc.bio_container_id
                                                  join prelab.biomaterial b
                                                    on b.biomaterial_id = bcn.biomaterial_id
                                                  join prelab.supplier sp
                                                    on sp.supplier_id = bc.supplier_id
                                                 where s.referral_id = @id
                                             """;
}